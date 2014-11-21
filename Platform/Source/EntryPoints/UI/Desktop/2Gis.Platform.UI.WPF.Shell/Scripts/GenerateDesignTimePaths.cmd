find /c "%1" %1Presentation\Blendability\DesignTimePaths.cs
if %errorlevel% equ 0 goto done
pushd %cd%
%1
cd %1
cd ..\..\..\..\..\..
set SolutionFolder=%cd%
popd
echo namespace DoubleGis.Platform.UI.WPF.Shell.Presentation.Blendability { static public class DesignTimePaths { public static readonly string LocalPath = @"%SolutionFolder%\CompositionRoots\Source\2Gis.Erm.UI.Desktop.WPF\bin\Debug";} } > %1Presentation\Blendability\DesignTimePaths.cs
exit 0
:done

