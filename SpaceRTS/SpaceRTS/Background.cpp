#include "stdafx.h"
#include "Background.h"

Background::Background() : m_gridQuad(nullptr)
{
}

Background::~Background()
{
}

void Background::onStart()
{
    m_gridQuad = getScene()->createManualObject();

    Ogre::Real sizeX = 512;
    Ogre::Real sizeZ = 512;

    Ogre::Real halfSizeX = sizeX / 2;
    Ogre::Real halfSizeZ = sizeZ / 2;

    Ogre::Real texSizeU = sizeX / 4;
    Ogre::Real texSizeV = sizeZ / 4;

    m_gridQuad->begin("background_grid", Ogre::RenderOperation::OperationType::OT_TRIANGLE_LIST);

    m_gridQuad->position(-halfSizeX, 0, -halfSizeZ);
    m_gridQuad->textureCoord(0, 0);
    m_gridQuad->position(halfSizeX, 0, -halfSizeZ);
    m_gridQuad->textureCoord(texSizeU, 0);
    m_gridQuad->position(-halfSizeX, 0, halfSizeZ);
    m_gridQuad->textureCoord(0, texSizeV);
    m_gridQuad->position(halfSizeX, 0, halfSizeZ);
    m_gridQuad->textureCoord(texSizeU, texSizeV);

    m_gridQuad->triangle(0, 3, 1);
    m_gridQuad->triangle(0, 2, 3);

    m_gridQuad->end();
    m_gridQuad->setVisible(true);

    m_gridQuad->setRenderQueueGroup(1);

    getScene()->getRootSceneNode()->attachObject(m_gridQuad);
}

void Background::onDestroy()
{
    if (m_gridQuad != nullptr)
    {
        getScene()->getRootSceneNode()->detachObject(m_gridQuad);
        getScene()->destroyManualObject(m_gridQuad);
        m_gridQuad = nullptr;
    }
}

void Background::tick(Ogre::Real timeDelta)
{
}