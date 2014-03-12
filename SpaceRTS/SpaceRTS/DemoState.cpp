#include "stdafx.h"
#include "DemoState.h"
#include "GameManager.h"
#include "HeadsUpDisplay.h"
#include "Grid.h"
#include "OverheadCameraMan.h"
#include "Planet.h"
#include "Ship.h"
#include "AsteroidField.h"
#include "Background.h"

using namespace std;

DemoState::DemoState(void)
{
}


DemoState::~DemoState(void)
{
}

void DemoState::onStart()
{
    //shared_ptr<Grid> grid = make_shared<Grid>(1000.0f, 10.0f);
    GameManager::getSingleton().addObject(make_shared<HeadsUpDisplay>());
    GameManager::getSingleton().addObject(make_shared<OverheadCameraMan>());
    GameManager::getSingleton().addObject(make_shared<Background>());
    //GameManager::getSingleton().addObject(grid);

    /*
    GameManager::getSingleton().addObject(make_shared<Ship>());
    GameManager::getSingleton().addObject(make_shared<Ship>());
    GameManager::getSingleton().addObject(make_shared<Ship>());
    shared_ptr<Planet> planet = make_shared<Planet>();
    planet->setPosition(Ogre::Vector3(10, 0, 0));
    GameManager::getSingleton().addObject(planet);
    */

    shared_ptr<AsteroidField> asteroidField = make_shared<AsteroidField>();
    asteroidField->setPosition(Ogre::Vector3(10, 0, 0));
    GameManager::getSingleton().addObject(asteroidField);

    getScene()->setAmbientLight(Ogre::ColourValue(0.2f, 0.2f, 0.3f));
    Ogre::Light* light = getScene()->createLight("MainLight");
    light->setPosition(20, 80, 50);

}