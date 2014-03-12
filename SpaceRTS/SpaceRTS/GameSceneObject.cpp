#include "stdafx.h"
#include "GameSceneObject.h"

GameSceneObject::GameSceneObject(void)
{
    this->m_sceneNode = getScene()->createSceneNode();
}

GameSceneObject::~GameSceneObject(void)
{
    if (m_sceneNode != nullptr)
    {
        // detach node from parent if it's attached
        Ogre::Node *parentNode = m_sceneNode->getParent();
        if (parentNode != nullptr)
        {
            parentNode->removeChild(m_sceneNode);
        }

        // destroy node
        getScene()->destroySceneNode(m_sceneNode);
    }
}
