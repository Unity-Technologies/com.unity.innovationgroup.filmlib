
# Unity Innovation Group Film Library

Library of code / tools created by the M&E Innovation Group at Unity.

The 'Master' branch is what the Sherman project used when it was released (built around Unity 18.4 LTS version)

The 'Dev' branch includes some new / experimental work.

There will also be additional unity-version-specific branches arriving soon as we start to port the code to the 19.x Tech Cycle.

# Using the Package


Open the manifest.json for your project and add the following entry to your list of dependencies

"com.newtonsoft.json": "https://github.com/Unity-Technologies/com.unity.innovationgroup.filmlib.git",

For example:

    "dependencies": {
        "com.unity.innovationgroup.filmlib": "https://github.com/Unity-Technologies/com.unity.innovationgroup.filmlib.git",
        "com.unity.ads": "2.0.8",
        "com.unity.analytics": "3.2.2",
        "com.unity.collab-proxy": "1.2.15",
        ...
        }

 You can also append the desired branch to the end of the URL, like so:

       "com.unity.innovationgroup.filmlib": "https://github.com/Unity-Technologies/com.unity.innovationgroup.filmlib.git#dev",

