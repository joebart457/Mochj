Run order:

BuildExec.bat
package-prod.mochj
=> upload "Package" folder to dropbox
=> copy remote link from dropbox to manifest.json (in the same folder as this readme)
=> upload manifest.json to dropbox
=> rename installers to MochjSetup-<VERSIONHERE>-global.EXE (or ...local.EXE)
=> upload to dropbox

IF local packaging:

BuildExec.bat
package-local.mochj
*lpm.mochj

*only run if (PackageManager.Update was run and the LPM module is no longer part of the manifest)