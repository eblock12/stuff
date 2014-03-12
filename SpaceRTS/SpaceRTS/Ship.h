#pragma once

#include "SelectionListener.h"

class Ship : public GameEntity, public SelectionListener
{

public:
    Ship(void);
    ~Ship(void);

    virtual void onStart();
    virtual void onDestroy();
    virtual void tick(Ogre::Real timeDelta);

    virtual void onSelect();
    virtual void onDeselect();
    virtual void onMoveOrder(Ogre::Vector3 destination);

private:
    bool m_selected;
    Ogre::Vector3 m_moveDesination;
    Ogre::Vector3 m_velocity;

    Ogre::Entity *m_reticleEntity;
    Ogre::SceneNode *m_reticleNode;
};

