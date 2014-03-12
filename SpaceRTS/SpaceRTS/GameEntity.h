#pragma once

#include "stdafx.h"
#include "gamesceneobject.h"

class GameEntity : public GameSceneObject
{
public:
    GameEntity(void);
    ~GameEntity(void);

protected:
    Ogre::Entity *m_entity;
};

