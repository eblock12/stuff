#include "stdafx.h"
#include "Planet.h"


Planet::Planet(void) : m_glowSpriteSet(nullptr)
{
}


Planet::~Planet(void)
{
}

void Planet::onStart()
{
    m_entity = getScene()->createEntity("planet.mesh");
    getScene()->getRootSceneNode()->addChild(m_sceneNode);
    m_sceneNode->scale(Ogre::Vector3(6));
    m_sceneNode->attachObject(m_entity);

    m_glowSpriteSet = getScene()->createBillboardSet(1);
    m_glowSpriteSet->setBillboardType(Ogre::BillboardType::BBT_ORIENTED_COMMON);
    m_glowSpriteSet->setDefaultDimensions(2.2,2.2);
    m_glowSpriteSet->createBillboard(Ogre::Vector3::ZERO);
    m_glowSpriteSet->setMaterialName("planet_glow");
    //m_sceneNode->attachObject(m_glowSpriteSet);
}

void Planet::onDestroy()
{
    if (m_glowSpriteSet != nullptr)
    {
        getScene()->destroyBillboardSet(m_glowSpriteSet);
        m_glowSpriteSet = nullptr;
    }
}

void Planet::tick(Ogre::Real timeDelta)
{
    m_sceneNode->yaw(Ogre::Radian(0.1 * timeDelta));
}