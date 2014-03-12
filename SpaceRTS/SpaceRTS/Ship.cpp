#include "stdafx.h"
#include "SelectionListener.h"
#include "HeadsUpDisplay.h"
#include "GameEntity.h"
#include "Ship.h"
#include "Math.h"

Ship::Ship(void) : m_reticleEntity(nullptr), m_reticleNode(nullptr)
{
}

Ship::~Ship(void)
{
}

void Ship::onStart()
{
    m_selected = false;
    m_moveDesination = Ogre::Vector3::ZERO;
    m_velocity = Ogre::Vector3::ZERO;

    m_entity = getScene()->createEntity("ship_scout.mesh");
    m_entity->setUserAny(Ogre::Any(Math::newSequentialID()));
    getScene()->getRootSceneNode()->addChild(m_sceneNode);
    m_sceneNode->attachObject(m_entity);

    HeadsUpDisplay::getSingleton().addSelectionListener(Ogre::any_cast<int>(m_entity->getUserAny()), this);
}

void Ship::onDestroy()
{
    HeadsUpDisplay::getSingleton().removeSelectionListener(Ogre::any_cast<int>(m_entity->getUserAny()));

    if (m_reticleEntity != nullptr)
    {
        getScene()->destroyEntity(m_reticleEntity);
        m_reticleEntity = nullptr;
    }
    if (m_reticleNode != nullptr)
    {
        Ogre::Node *parentNode = m_sceneNode->getParent();
        if (parentNode != nullptr)
        {
            parentNode->removeChild(m_sceneNode);
        }
        m_sceneNode = nullptr;
    }
}

void Ship::tick(Ogre::Real timeDelta)
{
    m_sceneNode->showBoundingBox(m_selected);
    

    Ogre::Quaternion orientation = m_sceneNode->getOrientation();

    if (m_moveDesination != Ogre::Vector3::ZERO)
    {
        // slerp orientation toward target direction
        Ogre::Vector3 currentDirection = orientation * Ogre::Vector3(0, 0, 1);
        Ogre::Vector3 desiredDirection = m_moveDesination - m_sceneNode->getPosition();
        desiredDirection.normalise();
        Ogre::Quaternion shortestArc = currentDirection.getRotationTo(desiredDirection);
        m_sceneNode->setOrientation(Ogre::Quaternion::Slerp(timeDelta * 2, orientation, shortestArc * orientation));

        // compute how far to turn to face target point
        Ogre::Real turnAngle = desiredDirection.dotProduct(currentDirection);
        if (fabs(turnAngle) >= 0.8)
        {
            // if not much turning left, start accelerating
            Ogre::Vector3 forward(0, 0, 70 * timeDelta);
            forward = m_sceneNode->getOrientation() * forward;
            m_velocity += forward;
        }

        // check if ship reached the target point
        Ogre::Real distanceSquared = m_sceneNode->getPosition().squaredDistance(m_moveDesination);
        if (distanceSquared < 3)
        {
            m_moveDesination = Ogre::Vector3::ZERO;
            m_reticleNode->setVisible(false);
        }
    }

    // spin reticle effect
    if ((m_reticleNode != nullptr) && (m_moveDesination != Ogre::Vector3::ZERO))
    {
        m_reticleNode->yaw(Ogre::Radian(timeDelta));
    }

    //Ogre::Vector3 bob(Vecor

    m_sceneNode->setPosition(m_sceneNode->getPosition() + m_velocity * timeDelta);
    
    m_velocity.x = Math::dampen(m_velocity.x, 5 * timeDelta);
    m_velocity.y = Math::dampen(m_velocity.y, 5 * timeDelta);
    m_velocity.z = Math::dampen(m_velocity.z, 5 * timeDelta);
}

void Ship::onSelect()
{
    m_selected = true;
}

void Ship::onDeselect()
{
    m_selected = false;
}

void Ship::onMoveOrder(Ogre::Vector3 destination)
{
    // create destination reticle objects if they don't exist yet
    if (m_reticleEntity == nullptr)
    {
        m_reticleEntity = getScene()->createEntity("reticle.mesh");
    }
    if (m_reticleNode == nullptr)
    {
        m_reticleNode = getScene()->getRootSceneNode()->createChildSceneNode();
        m_reticleNode->attachObject(m_reticleEntity);
    }

    m_reticleNode->setVisible(true);
    m_reticleNode->setPosition(destination);
    m_moveDesination = destination;
}