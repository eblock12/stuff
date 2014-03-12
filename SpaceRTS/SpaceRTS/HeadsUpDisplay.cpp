#include "stdafx.h"
#include "SelectionListener.h"
#include "HeadsUpDisplay.h"
#include "Math.h"

template<> HeadsUpDisplay *Ogre::Singleton<HeadsUpDisplay>::msSingleton = nullptr;

HeadsUpDisplay::HeadsUpDisplay()
    : m_cursorContainer(nullptr), m_overlay(nullptr), m_cursorX(0), m_cursorY(0), m_rayQuery(nullptr), m_mouseLeftLastDown(false),
      m_mouseRightLastDown(false), m_currentSelection(nullptr)
{
}

HeadsUpDisplay::~HeadsUpDisplay()
{
}

void HeadsUpDisplay::onStart()
{
    m_cursorContainer = static_cast<Ogre::OverlayContainer *>(Ogre::OverlayManager::getSingleton().createOverlayElement("Panel", "CursorPanel"));
    m_cursorContainer->setMaterialName("cursor");

    m_overlay = Ogre::OverlayManager::getSingleton().create("HeadsUpDisplayOverlay");
    m_overlay->setZOrder(200);
    m_overlay->add2D(m_cursorContainer);
    m_overlay->show();

    m_rayQuery = getScene()->createRayQuery(Ogre::Ray());
}

void HeadsUpDisplay::onDestroy()
{
    if (m_cursorContainer != nullptr)
    {
        Ogre::OverlayManager::getSingleton().destroyOverlayElement(m_cursorContainer);
        m_cursorContainer = nullptr;
    }
    if (m_overlay != nullptr)
    {
        Ogre::OverlayManager::getSingleton().destroy(m_overlay);
        m_overlay = nullptr;
    }
    if (m_rayQuery != nullptr)
    {
        getScene()->destroyQuery(m_rayQuery);
        m_rayQuery = nullptr;
    }
}

void HeadsUpDisplay::tick(Ogre::Real timeDelta)
{
    // window dimensions may have changed
    Application &app = Application::getSingleton();
    Ogre::Real windowWidth = (Ogre::Real)app.getWindowWidth();
    Ogre::Real windowHeight = (Ogre::Real)app.getWindowHeight();

    // get mouse cursor and update absolute position from relative change
    OIS::Mouse *mouse = app.getMouse();
    if (mouse != nullptr)
    {
        OIS::MouseState mouseState = mouse->getMouseState();
        m_cursorX += mouseState.X.rel;
        m_cursorY += mouseState.Y.rel;
        m_cursorX = Math::clamp(m_cursorX, 0, windowWidth);
        m_cursorY = Math::clamp(m_cursorY, 0, windowHeight);

        // normalize cursor position and size from 0 to 1
        Ogre::Real cursorX = m_cursorX / windowWidth;
        Ogre::Real cursorY = m_cursorY / windowHeight;
        m_cursorContainer->setPosition(cursorX, cursorY);
        m_cursorContainer->setWidth(CURSOR_WIDTH / windowWidth);
        m_cursorContainer->setHeight(CURSOR_HEIGHT / windowHeight);
        m_cursorContainer->show();

        // project cursor ray into scene and get query results
        if (!m_mouseLeftLastDown && mouseState.buttonDown(OIS::MouseButtonID::MB_Left))
        {
            Ogre::Ray mouseRay;
            getCamera()->getCameraToViewportRay(cursorX, cursorY, &mouseRay);
            m_rayQuery->setRay(mouseRay);
            m_objectFound = false;
            m_rayQuery->execute(this);

            if (!m_objectFound && (m_currentSelection != nullptr))
            {
                // user selected nothing so clear current selection
                m_currentSelection->onDeselect();
            }
        }

        if ((m_currentSelection != nullptr) && !m_mouseRightLastDown && mouseState.buttonDown(OIS::MouseButtonID::MB_Right))
        {
            Ogre::Ray mouseRay;
            getCamera()->getCameraToViewportRay(cursorX, cursorY, &mouseRay);
            Ogre::Plane plane(Ogre::Vector3(0, 1, 0), Ogre::Vector3::ZERO);
            auto intersectResult = mouseRay.intersects(plane);
            if (intersectResult.first)
            {
                Ogre::Vector3 destination = mouseRay.getPoint(intersectResult.second);
                m_currentSelection->onMoveOrder(destination);
            }
        }
        
        m_mouseLeftLastDown = mouseState.buttonDown(OIS::MouseButtonID::MB_Left);
        m_mouseRightLastDown = mouseState.buttonDown(OIS::MouseButtonID::MB_Right);
    }
    else
    {
        m_cursorContainer->hide();
    }
}

void HeadsUpDisplay::addSelectionListener(int objectID, SelectionListener *listener)
{
    m_selectionListenerMap.insert(std::make_pair<int, SelectionListener *>(std::move(objectID), std::move(listener)));
}

void HeadsUpDisplay::removeSelectionListener(int objectID)
{
    m_selectionListenerMap.erase(objectID);
}

bool HeadsUpDisplay::queryResult(Ogre::SceneQuery::WorldFragment *fragment, Ogre::Real distance)
{
    return false;
}

bool HeadsUpDisplay::queryResult(Ogre::MovableObject *obj, Ogre::Real distance)
{
    if (obj->getUserAny().isEmpty())
    {
        // not tagged with an ID, keep searching
        return true;
    }

    auto result = m_selectionListenerMap.find(Ogre::any_cast<int>(obj->getUserAny()));
    if (result != m_selectionListenerMap.end())
    {
        SelectionListener *newSelection = (*result).second;

        if ((m_currentSelection != nullptr) && (newSelection != m_currentSelection))
        {
            // deselect old selection
            m_currentSelection->onDeselect();
        }

        // found result, invoke click listener
        newSelection->onSelect();
        m_currentSelection = newSelection;
        m_objectFound = true;

        // stop searching for more objects
        return false;
    }
    else
    {
        // no match found, continue search
        return true;
    }

    return true;
}