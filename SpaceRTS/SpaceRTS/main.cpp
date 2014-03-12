#include "stdafx.h"
#include "Application.h"

using namespace std;

int main()
{
    int result = 0;

    try
    {
        Application app;
        app.run();
    }
    catch (Ogre::Exception &e)
    {
        cout << "Unhandled Ogre exception encountered: " << e.what() << endl;
        result = 1;
    }
    catch (exception &e)
    {
        cout << "Unhandled exception encountered: "  << e.what() << endl;
        result = 1;
    }

    //getchar();

    return result;
}