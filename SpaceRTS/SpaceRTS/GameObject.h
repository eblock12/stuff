#pragma once

#include "Application.h"

class GameObject
{
public:
    GameObject();
    ~GameObject();

    void destroy();

    inline bool isDestroyed() { return m_destroyed; }

    virtual void onStart();
    virtual void onDestroy();
    virtual void tick(Ogre::Real timeDelta);

protected:
    // convenience methods
    inline Ogre::Camera *getCamera() { return Application::getSingleton().getCamera(); }
    inline Ogre::SceneManager *getScene() { return Application::getSingleton().getScene(); }

private:
    bool m_destroyed;
};