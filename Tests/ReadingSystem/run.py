"""Run reader logic checks against actual scripts with Unity API stubs (.NET 10 SDK).
This does not replace Unity compilation, Play Mode, physics or visual validation.
"""
from pathlib import Path
import os
import re
import shutil
import subprocess
import tempfile
import xml.etree.ElementTree as ET

TESTS = Path(__file__).resolve().parent
REPO = TESTS.parent.parent
PROJECT = REPO / "mu's adventure"


def check_scene():
    scene = (PROJECT / "Assets/Scenes/chapter1.unity").read_text()
    entries = list(re.finditer(r'^--- !u!(\d+) &(\d+)\n(.*?)(?=^---|\Z)', scene, re.M | re.S))
    blocks = {int(m[2]): m[3] for m in entries}
    assert len(blocks) == len(entries), 'Duplicate scene IDs'
    for match in re.finditer(r'\{fileID: (\d+)\}', scene):
        ref = int(match[1])
        assert ref == 0 or ref in blocks, f'Missing scene reference {ref}'
    for ref, body in blocks.items():
        if not body.startswith(('Transform:', 'RectTransform:')):
            continue
        parent = int(re.search(r'm_Father: \{fileID: (\d+)\}', body)[1])
        if parent:
            siblings = re.search(r'  m_Children:(.*?)(?=  m_Father:)', blocks[parent], re.S)[1]
            assert f'{{fileID: {ref}}}' in siblings, f'Parent does not list {ref}'
        children = re.search(r'  m_Children:(.*?)(?=  m_Father:)', body, re.S)[1]
        for child in re.findall(r'fileID: (\d+)', children):
            assert f'm_Father: {{fileID: {ref}}}' in blocks[int(child)]
    assert 'm_IsTrigger: 1' in blocks[1238555207]
    assert 'c49f840164654a47a14d0f5de6e04a3b' in blocks[1238555206]
    assert 'interactionPrompt: {fileID: 2109000002}' in blocks[112976622]
    for block in (112976622, 1238555206, 1238555208, 2109000002):
        for guid in re.findall(r'guid: ([0-9a-f]{32})', blocks[block]):
            if guid.startswith('0000000000000000'):
                continue  # Unity built-in resource.
            # Package scripts (TMP/UI) are resolved by Unity; check local content/script assets below.
            if guid in ('f4688fdb7df04437aeb418b961361dc5',):
                continue
            assert any(f'guid: {guid}' in p.read_text(errors='ignore')
                       for p in (PROJECT / 'Assets').rglob('*.meta')), f'Missing asset {guid}'
    print('PASS scene references, hierarchy, blackboard document and reading prompt', flush=True)


def main():
    check_scene()
    dotnet = shutil.which('dotnet')
    if not dotnet:
        candidate = Path('/usr/local/share/dotnet/dotnet')
        dotnet = str(candidate) if candidate.exists() else None
    if not dotnet:
        raise SystemExit('Install .NET 10 SDK to run logic checks; scene checks passed.')
    with tempfile.TemporaryDirectory(prefix='mu-reading-checks-') as temp:
        work = Path(temp)
        project = ET.Element('Project', Sdk='Microsoft.NET.Sdk')
        props = ET.SubElement(project, 'PropertyGroup')
        for key, value in [('OutputType', 'Exe'), ('TargetFramework', 'net10.0'),
                           ('Nullable', 'disable'), ('EnableDefaultCompileItems', 'false')]:
            ET.SubElement(props, key).text = value
        items = ET.SubElement(project, 'ItemGroup')
        for name in ['FileUIManager.CS', 'FileDocument.cs', 'FileInteractable.cs', 'MinigameCameraSwitcher.cs']:
            ET.SubElement(items, 'Compile', Include=str(PROJECT / 'Assets/Script' / name))
        for name in ['UnityStubs.cs', 'Program.cs']:
            ET.SubElement(items, 'Compile', Include=str(TESTS / name))
        project_file = work / 'ReadingChecks.csproj'
        ET.ElementTree(project).write(project_file, encoding='unicode')
        (work / 'NuGet.Config').write_text(
            '<configuration><packageSources><clear /></packageSources></configuration>')
        env = dict(os.environ, DOTNET_CLI_HOME=str(work / 'home'),
                   DOTNET_CLI_TELEMETRY_OPTOUT='1', DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1',
                   DOTNET_NOLOGO='1', DOTNET_GENERATE_ASPNET_CERTIFICATE='false')
        subprocess.run([dotnet, 'run', '--project', str(project_file), '--verbosity', 'quiet'],
                       env=env, check=True)


if __name__ == '__main__':
    main()
