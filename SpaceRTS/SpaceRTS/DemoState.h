#pragma once

#include "gameobject.h"

class DemoState : public GameObject
{
public:
    DemoState(void);
    ~DemoState(void);

    virtual void onStart();
};

