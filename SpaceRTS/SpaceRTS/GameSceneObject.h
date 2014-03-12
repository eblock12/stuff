#pragma once

#include "gameobject.h"

class GameSceneObject : public GameObject
{
public:
    GameSceneObject(void);
    ~GameSceneObject(void);

    inline Ogre::Vector3 getPosition() { return m_sceneNode->getPosition(); }
    inline void setPosition(Ogre::Vector3 &v) { return m_sceneNode->setPosition(v); }

protected:
    Ogre::SceneNode *m_sceneNode;
};

