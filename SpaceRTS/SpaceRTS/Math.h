#pragma once

#include "stdafx.h"

class Math
{
public:
    inline static int newSequentialID()
    {
        static int id = 0;
        return id++;
    }

    inline static Ogre::Real clamp(Ogre::Real a, Ogre::Real min, Ogre::Real max)
    {
        if (a < min)
        {
            return min;
        }
        if (a > max)
        {
            return max;
        }
        return a;
    }

    inline static Ogre::Real dampen(Ogre::Real a, Ogre::Real amount)
    {
        if (a < -0.01f)
        {
            a -= a * amount;
        }
        else if (a > 0.01f)
        {
            a -= a * amount;
        }
        else
        {
            a = 0;
        }

        return a;
    }
};