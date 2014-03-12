#pragma once

#include "gameobject.h"
#include "SelectionListener.h"

class HeadsUpDisplay : public GameObject, public Ogre::Singleton<HeadsUpDisplay>, public Ogre::RaySceneQueryListener
{
public:
    HeadsUpDisplay(void);
    ~HeadsUpDisplay(void);

    void addSelectionListener(int objectID, SelectionListener *listener);
    void removeSelectionListener(int objectID);

    virtual void onStart();
    virtual void onDestroy();
    virtual void tick(Ogre::Real timeDelta);

protected:
    virtual bool queryResult(Ogre::SceneQuery::WorldFragment *fragment, Ogre::Real distance);
    virtual bool queryResult(Ogre::MovableObject *obj, Ogre::Real distance);

private:
    std::unordered_map<int, SelectionListener *> m_selectionListenerMap;
    SelectionListener *m_currentSelection;
    bool m_objectFound;

    Ogre::RaySceneQuery *m_rayQuery;
    Ogre::Overlay *m_overlay;
    Ogre::OverlayContainer *m_cursorContainer;
    int m_cursorX;
    int m_cursorY;
    bool m_mouseLeftLastDown;
    bool m_mouseRightLastDown;
};

static const Ogre::Real CURSOR_WIDTH = 32;
static const Ogre::Real CURSOR_HEIGHT = 32;