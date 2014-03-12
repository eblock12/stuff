#pragma once

class SelectionListener
{
public:
    virtual ~SelectionListener() { }

    virtual void onSelect() = 0;
    virtual void onDeselect() = 0;
    virtual void onMoveOrder(Ogre::Vector3 destination) = 0;
};