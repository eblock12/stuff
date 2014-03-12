#pragma once

#include <vector>
#include "gamesceneobject.h"

class AsteroidField : public GameSceneObject
{
public:
    AsteroidField();
    ~AsteroidField();

    virtual void onStart();
    virtual void onDestroy();
    virtual void tick(Ogre::Real timeDelta);

private:
    std::vector<Ogre::Entity *> m_asteroidEntities;
    std::vector<Ogre::SceneNode *> m_asteroidNodes;
};

