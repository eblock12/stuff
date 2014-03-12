#pragma once

#include "gameentity.h"

class Grid : public GameSceneObject
{
public:
    Grid(Ogre::Real length, Ogre::Real divisions);
    ~Grid(void);

    virtual void onStart();

private:
    Ogre::ManualObject *m_gridObject;
};

