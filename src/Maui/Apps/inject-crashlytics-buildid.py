import os, zipfile, glob, shutil, sys

build_id = sys.argv[1]
obj = sys.argv[2].replace('\\', '/')
content = ('version=18.3.2\nclient=firebase-crashlytics\nfirebase-crashlytics_client=18.3.2\nbuild_id=' + build_id + '\n').encode()

patched = 0
for jar_path in glob.glob(os.path.join(obj, 'lp', '*', 'jl', 'classes.jar')):
    try:
        with zipfile.ZipFile(jar_path, 'r') as z:
            if 'firebase-crashlytics.properties' not in z.namelist():
                continue
        tmp = jar_path + '.tmp'
        with zipfile.ZipFile(jar_path, 'r') as zin, zipfile.ZipFile(tmp, 'w', zipfile.ZIP_STORED) as zout:
            for item in zin.infolist():
                data = content if item.filename == 'firebase-crashlytics.properties' else zin.read(item.filename)
                zout.writestr(item, data)
        shutil.move(tmp, jar_path)
        print('Patched: ' + jar_path)
        patched += 1
    except Exception as e:
        print('Error on ' + jar_path + ': ' + str(e), file=sys.stderr)
        sys.exit(1)

if patched == 0:
    print('firebase-crashlytics.properties not found in any lp JAR - skipping')
