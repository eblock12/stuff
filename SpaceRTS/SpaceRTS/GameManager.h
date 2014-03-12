#pragma once

#include "GameObject.h"

class GameManager : public Ogre::Singleton<GameManager>
{
public:
    GameManager(void);
    ~GameManager(void);

    void addObject(std::shared_ptr<GameObject> obj);
    void tick(Ogre::Real timeDelta);

private:
    std::list<std::shared_ptr<GameObject>> m_startingObjectList;
    std::list<std::shared_ptr<GameObject>> m_runningObjectList;
    std::list<std::shared_ptr<GameObject>> m_stoppingObjectList;
};

