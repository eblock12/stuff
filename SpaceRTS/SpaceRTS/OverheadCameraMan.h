#pragma once

#include "gameobject.h"

class OverheadCameraMan : public GameObject
{
public:
    OverheadCameraMan();
    ~OverheadCameraMan();

    virtual void onStart();
    virtual void onDestroy();
    virtual void tick(Ogre::Real timeDelta);

private:
    Ogre::Radian m_yaw;
    Ogre::Radian m_pitch;
    Ogre::Real m_radius;

    Ogre::Vector3 m_eye;
    Ogre::Vector3 m_focus;
    Ogre::Vector3 m_right;
    Ogre::Vector3 m_look;
};

