#include "stdafx.h"

#include "OverheadCameraMan.h"

OverheadCameraMan::OverheadCameraMan()
{
}

OverheadCameraMan::~OverheadCameraMan()
{
}

void OverheadCameraMan::onStart()
{
    // orbit angles around focus point
    m_yaw = m_pitch = Ogre::Degree(45);

    // distance from focus point
    m_radius = 80;

    // focus on world origin
    m_focus = Ogre::Vector3::ZERO;
}

void OverheadCameraMan::onDestroy()
{
}

void OverheadCameraMan::tick(Ogre::Real timeDelta)
{
    Ogre::Camera *camera = getCamera();
    
    // restrict focus movement to XZ plane
    m_right.y = m_look.y = 0;
    m_right.normalise();
    m_look.normalise();

    // get keyboard state
    OIS::Keyboard *keyboard = Application::getSingleton().getKeyboard();
    if (keyboard != nullptr)
    {
        const Ogre::Real ScrollSpeed = 4;
        const Ogre::Real ZoomSpeed = 40;

        // scroll camera by translating focus point, zoom by changing orbit radius
        if (keyboard->isKeyDown(OIS::KeyCode::KC_LEFT) || keyboard->isKeyDown(OIS::KeyCode::KC_A))
        {
            m_focus -= m_right * timeDelta * (ScrollSpeed + m_radius * 0.2f);
        }
        if (keyboard->isKeyDown(OIS::KeyCode::KC_RIGHT) || keyboard->isKeyDown(OIS::KeyCode::KC_D))
        {
            m_focus += m_right * timeDelta * (ScrollSpeed + m_radius * 0.2f);
        }
        if (keyboard->isKeyDown(OIS::KeyCode::KC_UP) || keyboard->isKeyDown(OIS::KeyCode::KC_W))
        {
            m_focus -= m_look * timeDelta * (ScrollSpeed + m_radius * 0.2f);
        }
        if (keyboard->isKeyDown(OIS::KeyCode::KC_DOWN) || keyboard->isKeyDown(OIS::KeyCode::KC_S))
        {
            m_focus += m_look * timeDelta * (ScrollSpeed + m_radius * 0.2f);
        }
        if (keyboard->isKeyDown(OIS::KeyCode::KC_SUBTRACT))
        {
            m_radius += ZoomSpeed * timeDelta;
        }
        if (keyboard->isKeyDown(OIS::KeyCode::KC_ADD))
        {
            m_radius -= ZoomSpeed * timeDelta;
        }
    }

    // calculate eye position orbiting around the focus point
    Ogre::Real sideRadius = m_radius * Ogre::Math::Cos(m_pitch);
    Ogre::Real height = m_radius * Ogre::Math::Sin(m_pitch);
    m_eye = Ogre::Vector3(
        m_focus.x + sideRadius * Ogre::Math::Cos(m_yaw),
        m_focus.y + height,
        m_focus.z + sideRadius * Ogre::Math::Sin(m_yaw));

    // update actual camera
    camera->setPosition(m_eye);
    camera->lookAt(m_focus);

    // extract right and "look at" vectors
    const Ogre::Matrix4 &viewMatrix = camera->getViewMatrix();
    m_right.x = viewMatrix[0][0];
    m_right.y = viewMatrix[0][1];
    m_right.z = viewMatrix[0][2];
    m_look.x = viewMatrix[2][0];
    m_look.y = viewMatrix[2][1];
    m_look.z = viewMatrix[2][2];
}