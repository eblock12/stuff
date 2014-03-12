#include "stdafx.h"
#include "Grid.h"

Grid::Grid(Ogre::Real length, Ogre::Real step)
{
    Ogre::Real halfLength = length / 2;

    m_gridObject = getScene()->createManualObject("GridObject");
    m_gridObject->setDynamic(false);

    m_gridObject->begin("Debug/GridLine", Ogre::RenderOperation::OT_LINE_LIST);
    for (Ogre::Real x = -halfLength; x <= halfLength; x += step)
    {
        m_gridObject->position(x, 0, halfLength);
        m_gridObject->position(x, 0, -halfLength);
    }
    for (Ogre::Real z = -halfLength; z <= halfLength; z += step)
    {
        m_gridObject->position(halfLength, 0, z);
        m_gridObject->position(-halfLength, 0, z);
    }
    m_gridObject->end();
    m_gridObject->setVisible(true);
}

Grid::~Grid(void)
{
    if (m_gridObject != nullptr)
    {
        getScene()->destroyManualObject(m_gridObject);
        m_gridObject = nullptr;
    }
}

void Grid::onStart()
{
    // attach grid renderable to the scene node and add to the root of the scene
    getScene()->getRootSceneNode()->addChild(m_sceneNode);
    m_sceneNode->attachObject(m_gridObject);
}