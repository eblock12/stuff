#include "stdafx.h"
#include "GameObject.h"

GameObject::GameObject() : m_destroyed(false)
{
}

GameObject::~GameObject()
{
}

void GameObject::destroy()
{
    m_destroyed = true;
}

void GameObject::onStart()
{

}

void GameObject::onDestroy()
{
}

void GameObject::tick(Ogre::Real timeDelta)
{
}