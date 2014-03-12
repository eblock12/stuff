#include "stdafx.h"
#include "Application.h"
#include "GameManager.h"
#include "DemoState.h"

using namespace std;

template<> Application *Ogre::Singleton<Application>::msSingleton = nullptr;

Application::Application() :
    m_ogreRoot(unique_ptr<Ogre::Root>(new Ogre::Root("" /*pluginFileName*/, "" /*configFileName*/, "Ogre.log"))),
    m_gameMgr(unique_ptr<GameManager>(new GameManager())),
    m_window(nullptr),
    m_scene(nullptr),
    m_camera(nullptr),
    m_inputManager(nullptr),
    m_keyboard(nullptr),
    m_mouse(nullptr),
    m_debugCameraMan(nullptr)
{
    // load any Ogre plugins needed
    vector<Ogre::String> pluginList;
    //pluginList.push_back("RenderSystem_GL");
    pluginList.push_back("RenderSystem_Direct3D9");
    pluginList.push_back("Plugin_OctreeSceneManager");
    pluginList.push_back("Plugin_ParticleFX");
    for (auto &pluginName : pluginList)
    {
        if (OGRE_DEBUG_MODE)
        {
            // plugins are named differently in debug builds
            pluginName.append("_d");
        }
        m_ogreRoot->loadPlugin(pluginName);
    }

    // select a RenderSystem to use, only one is expected to be found
    const Ogre::RenderSystemList &renderSystemList = m_ogreRoot->getAvailableRenderers();
    if (renderSystemList.empty())
    {
        throw new exception("Failed to initialize Ogre because no render system was found");
    }
    m_ogreRoot->setRenderSystem(renderSystemList[0]);

    // initialize the Ogre root
    m_ogreRoot->initialise(false /*autoCreateWindow*/, "" /*windowTitle*/, "" /*customCapabilitiesConfig*/);

    // create a window
    Ogre::NameValuePairList windowParams;
    windowParams["FSAA"] = "0";
    windowParams["vsync"] = "true";
    m_window = m_ogreRoot->createRenderWindow("RENDERIGN WINDOW", 1280, 720, false /*fullScreen*/, &windowParams);
    windowResized(m_window); // call resize event for initial window creation

    // attach event listeners
    m_ogreRoot->addFrameListener(this);
    Ogre::WindowEventUtilities::addWindowEventListener(m_window, this);

    // create a "generic" scene manager, this gives an octree manager if the octree plugin is loaded
    m_scene = m_ogreRoot->createSceneManager(Ogre::SceneType::ST_GENERIC);

    // setup the camera
    m_camera = m_scene->createCamera("SceneCamera");
    m_camera->setPosition(20, 40, 20);
    m_camera->lookAt(0, 0, 0);
    m_camera->setNearClipDistance(0.1f);
    m_camera->setFarClipDistance(10000);
    //m_debugCameraMan = make_shared<OgreBites::SdkCameraMan>(m_camera);
    
    // create a single viewport for the entire window
    Ogre::Viewport *viewport = m_window->addViewport(m_camera);
    viewport->setBackgroundColour(Ogre::ColourValue::Black);
    m_camera->setAspectRatio(Ogre::Real(viewport->getActualWidth()) / Ogre::Real(viewport->getActualHeight()));

    // setup default resource group
    Ogre::ResourceGroupManager &resourceGroupMgr = Ogre::ResourceGroupManager::getSingleton();
    resourceGroupMgr.addResourceLocation("resources/general", "FileSystem", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME, false /*recursive*/);
    resourceGroupMgr.initialiseAllResourceGroups();

    // initialize OIS and input devices
    size_t winHandle = 0;
    m_window->getCustomAttribute("WINDOW", &winHandle);
    m_inputManager = OIS::InputManager::createInputSystem(winHandle);
    m_keyboard = static_cast<OIS::Keyboard *>(m_inputManager->createInputObject(OIS::OISKeyboard, true /*bufferMode*/));
    m_mouse = static_cast<OIS::Mouse *>(m_inputManager->createInputObject(OIS::OISMouse, true /*bufferMode*/));
    m_keyboard->setEventCallback(this);
    m_mouse->setEventCallback(this);
    
    // bring up the initial game object
    GameManager::getSingleton().addObject(make_shared<DemoState>());
}


Application::~Application()
{
    // make sure game manager is released before the Application goes down
    m_gameMgr.reset();

    if (m_window)
    {
        // remove event listeners
        Ogre::WindowEventUtilities::removeWindowEventListener(m_window, this);
        windowClosed(m_window);
    }
}

void Application::run()
{
    // run loop until window is closed
    Ogre::LogManager::getSingleton().logMessage("Started the message loop");
    while (m_window && !m_window->isClosed())
    {
        // pump window messages to handle events properly
        Ogre::WindowEventUtilities::messagePump();

        if (m_window->isActive())
        {
            // render a single frame
            if (!m_ogreRoot->renderOneFrame())
            {
                break;
            }
        }
        else
        {
            // if window isn't active then yield the CPU for awhile
            Sleep(500);
        }
    }
    Ogre::LogManager::getSingleton().logMessage("Exited from message loop");
}

bool Application::mouseMoved(const OIS::MouseEvent &arg )
{
    if (m_debugCameraMan != nullptr)
    {
        m_debugCameraMan->injectMouseMove(arg);
    }

    return true;
}

bool Application::mousePressed(const OIS::MouseEvent &arg, OIS::MouseButtonID id)
{
    if (m_debugCameraMan != nullptr)
    {
        m_debugCameraMan->injectMouseDown(arg, id);
    }

    return true;
}

bool Application::mouseReleased(const OIS::MouseEvent &arg, OIS::MouseButtonID id)
{
    if (m_debugCameraMan != nullptr)
    {
        m_debugCameraMan->injectMouseUp(arg, id);
    }

    return true;
}

bool Application::keyPressed(const OIS::KeyEvent &arg)
{
    if (m_debugCameraMan != nullptr)
    {
        m_debugCameraMan->injectKeyDown(arg);
    }

    return true;
}

bool Application::keyReleased(const OIS::KeyEvent &arg)
{
    if (m_debugCameraMan != nullptr)
    {
        m_debugCameraMan->injectKeyUp(arg);
    }

    return true;
}

bool Application::frameRenderingQueued(const Ogre::FrameEvent &evt)
{
    // capture input device state
    if (m_keyboard && m_mouse)
    {
        m_keyboard->capture();
        m_mouse->capture();

        if (m_keyboard->isKeyDown(OIS::KC_ESCAPE))
        {
            // abort render loop
            return false;
        }
    }

    Ogre::Real tickLength = min<Ogre::Real>(evt.timeSinceLastFrame, 1);
    GameManager::getSingleton().tick(tickLength);

    if (m_debugCameraMan != nullptr)
    {
        m_debugCameraMan->frameRenderingQueued(evt);
    }

    return true;
}

void Application::windowResized(Ogre::RenderWindow *window)
{
    m_windowWidth = window->getWidth();
    m_windowHeight = window->getHeight();
}

void Application::windowClosed(Ogre::RenderWindow *window)
{
    // ensure input systems are destroyed before window is closed
    if (m_inputManager)
    {
        if (m_keyboard)
        {
            m_keyboard->setEventCallback(nullptr);
            m_inputManager->destroyInputObject(m_keyboard);
            m_keyboard = nullptr;
        }
        if (m_mouse)
        {
            m_mouse->setEventCallback(nullptr);
            m_inputManager->destroyInputObject(m_mouse);
            m_mouse = nullptr;
        }

        OIS::InputManager::destroyInputSystem(m_inputManager);
        m_inputManager = nullptr;
    }
}