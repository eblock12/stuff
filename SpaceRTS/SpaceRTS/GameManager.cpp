#include "stdafx.h"
#include "GameManager.h"

template<> GameManager *Ogre::Singleton<GameManager>::msSingleton = nullptr;

GameManager::GameManager(void)
{
}

GameManager::~GameManager(void)
{
    for (auto objIterator = m_runningObjectList.rbegin(); objIterator != m_runningObjectList.rend(); ++objIterator)
    {
        (*objIterator)->onDestroy();
    }
}

void GameManager::addObject(std::shared_ptr<GameObject> obj)
{
    // schedule object to start running on next frame update
    m_startingObjectList.push_back(obj);
}

void GameManager::tick(Ogre::Real timeDelta)
{
    // process objects that are scheduled to start running
    if (!m_startingObjectList.empty())
    {
        for (auto obj : m_startingObjectList)
        {
            // object started, move to running list
            obj->onStart();
            m_runningObjectList.push_back(obj);
        }
        m_startingObjectList.clear();
    }

    // process objects that are currently running
    for (auto objIterator = m_runningObjectList.begin(); objIterator != m_runningObjectList.end();)
    {
        auto obj = *objIterator;

        // if object is flagged as destroyed, remove from running list
        if (obj->isDestroyed())
        {
            obj->onDestroy();
            objIterator = m_runningObjectList.erase(objIterator);
        }
        else
        {
            // object is running so elapse time for it
            obj->tick(timeDelta);

            objIterator++;
        }
    }
}