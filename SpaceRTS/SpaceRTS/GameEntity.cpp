#include "stdafx.h"
#include "Application.h"
#include "GameEntity.h"

GameEntity::GameEntity(void) : m_entity(nullptr)
{
}

GameEntity::~GameEntity(void)
{
    if (m_entity != nullptr)
    {
        getScene()->destroyEntity(m_entity);
    }
}
