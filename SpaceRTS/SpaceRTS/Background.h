#pragma once
#include "gameobject.h"

class Background : public GameObject
{
public:
    Background();
    ~Background();

    virtual void onStart();
    virtual void onDestroy();
    virtual void tick(Ogre::Real timeDelta);

private:
    Ogre::ManualObject *m_gridQuad;
};

