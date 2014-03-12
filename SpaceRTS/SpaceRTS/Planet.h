#pragma once

#include "gameentity.h"

class Planet : public GameEntity
{
public:
    Planet(void);
    ~Planet(void);

    virtual void onStart();
    virtual void onDestroy();
    virtual void tick(Ogre::Real timeDelta);

private:
    Ogre::BillboardSet *m_glowSpriteSet;
};

