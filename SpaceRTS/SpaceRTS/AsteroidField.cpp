#include "stdafx.h"
#include "AsteroidField.h"

AsteroidField::AsteroidField() : m_asteroidEntities(), m_asteroidNodes()
{
}

AsteroidField::~AsteroidField()
{
}

void AsteroidField::onStart()
{
    //Ogre::Entity *testAsteroid = getScene()->createEntity("rock1.mesh");
    //m_sceneNode->attachObject(testAsteroid);

    // temp hack hack
    m_sceneNode->setPosition(40, 0, 40);

    const int asteroidCount = 15;

    //for (Ogre::Real slotX = -halfSizeX; slotX <= halfSizeX; slotX += spacingStep)
    //{
    //    for (Ogre::Real slotZ = -halfSizeZ; slotZ <= halfSizeZ; slotZ += spacingStep)
    //    {
    //        if (Ogre::Math::UnitRandom() <= density)
    //        {
    //            Ogre::Entity *asteroidEntity = getScene()->createEntity("rock1.mesh");
    //            m_asteroidEntities.push_back(asteroidEntity);

    //            Ogre::SceneNode *asteroidNode = getScene()->createSceneNode();
    //            asteroidNode->setPosition(slotX, 0, slotZ);
    //            asteroidNode->translate(Ogre::Math::RangeRandom(-spacingStep/3, spacingStep/3), 0, Ogre::Math::RangeRandom(-spacingStep/3, spacingStep/3));
    //            asteroidNode->pitch(Ogre::Degree(Ogre::Math::RangeRandom(0, 360)));
    //            asteroidNode->yaw(Ogre::Degree(Ogre::Math::RangeRandom(0, 360)));
    //            asteroidNode->attachObject(asteroidEntity);
    //            m_sceneNode->addChild(asteroidNode);
    //            m_asteroidNodes.push_back(asteroidNode);
    //        }
    //    }
    //}

    for (int i = 0; i < asteroidCount; i++)
    {
        Ogre::Entity *asteroidEntity = getScene()->createEntity("rock1.mesh");
        m_asteroidEntities.push_back(asteroidEntity);

        Ogre::SceneNode *asteroidNode = getScene()->createSceneNode();

        const Ogre::Real radius = 0.5f;
        Ogre::Radian angle = Ogre::Degree(Ogre::Math::RangeRandom(0, 360));

        asteroidNode->setPosition(Ogre::Math::Cos(angle) * radius, 0, Ogre::Math::Sin(angle) * radius);
        asteroidNode->pitch(Ogre::Degree(Ogre::Math::RangeRandom(0, 360)));
        asteroidNode->yaw(Ogre::Degree(Ogre::Math::RangeRandom(0, 360)));
        asteroidNode->attachObject(asteroidEntity);
        m_sceneNode->addChild(asteroidNode);
        m_asteroidNodes.push_back(asteroidNode);
    }

    // repel for several iterations
    const Ogre::Real repelForce = 10;
    for (int i = 0; i < 10; i++)
    {
        for (Ogre::SceneNode *asteroidNode : m_asteroidNodes)
        {
            for (Ogre::SceneNode *otherNode : m_asteroidNodes)
            {
                if (otherNode != asteroidNode)
                {
                    Ogre::Vector3 directionVector = asteroidNode->getPosition() - otherNode->getPosition();
                    Ogre::Real distanceSquared = directionVector.squaredLength();
                    directionVector.normalise();

                    Ogre::Real force = repelForce / distanceSquared;
                    if (force > 1)
                    {
                        force = 1;
                    }
                    asteroidNode->translate(directionVector * force);
                }
            }
        }
    }

    // perturb
    const Ogre::Real perturbForce = 1.5;
    for (Ogre::SceneNode *asteroidNode : m_asteroidNodes)
    {
        Ogre::Radian perturbAngle = Ogre::Degree(Ogre::Math::RangeRandom(0, 360));
        asteroidNode->translate(Ogre::Math::Cos(perturbAngle) * perturbForce, 0, Ogre::Math::Sin(perturbAngle) * perturbForce);
    }

    getScene()->getRootSceneNode()->addChild(m_sceneNode);
}

void AsteroidField::onDestroy()
{
    m_sceneNode->removeAndDestroyAllChildren();
    m_asteroidNodes.clear();

    for (Ogre::Entity *asteroidEntity : m_asteroidEntities)
    {
        getScene()->destroyEntity(asteroidEntity);
    }
    m_asteroidEntities.clear();
}

void AsteroidField::tick(Ogre::Real timeDelta)
{
    for (Ogre::SceneNode *asteroidNode : m_asteroidNodes)
    {
        asteroidNode->roll(Ogre::Degree(40 * timeDelta));
        asteroidNode->yaw(Ogre::Degree(20 * timeDelta));
    }
}