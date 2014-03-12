#pragma once

class GameManager;

class Application : public Ogre::Singleton<Application>, public Ogre::WindowEventListener, public Ogre::FrameListener, public OIS::MouseListener, public OIS::KeyListener
{
public:
    Application();
    ~Application();

    inline Ogre::Camera *getCamera() { return m_camera; }
    inline Ogre::SceneManager *getScene() { return m_scene; }
    inline OIS::Mouse *getMouse() { return m_mouse; }
    inline OIS::Keyboard *getKeyboard() { return m_keyboard; }

    inline int getWindowWidth() { return m_windowWidth; }
    inline int getWindowHeight() { return m_windowHeight; }

    void run();

protected:
    virtual bool frameRenderingQueued(const Ogre::FrameEvent &evt);
    virtual void windowResized(Ogre::RenderWindow *window);
    virtual void windowClosed(Ogre::RenderWindow *window);

    virtual bool mouseMoved(const OIS::MouseEvent &arg );
    virtual bool mousePressed(const OIS::MouseEvent &arg, OIS::MouseButtonID id);
    virtual bool mouseReleased(const OIS::MouseEvent &arg, OIS::MouseButtonID id);

	virtual bool keyPressed(const OIS::KeyEvent &arg);
	virtual bool keyReleased(const OIS::KeyEvent &arg);
    
private:
    std::unique_ptr<GameManager> m_gameMgr;

    std::unique_ptr<Ogre::Root> m_ogreRoot;
    Ogre::RenderWindow *m_window;
    Ogre::SceneManager *m_scene;
    Ogre::Camera *m_camera;

    OIS::InputManager *m_inputManager;
    OIS::Mouse *m_mouse;
    OIS::Keyboard *m_keyboard;

    std::shared_ptr<OgreBites::SdkCameraMan> m_debugCameraMan;

    int m_windowWidth;
    int m_windowHeight;
};

