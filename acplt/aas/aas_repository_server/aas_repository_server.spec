# -*- mode: python ; coding: utf-8 -*-


block_cipher = None


# AAS Repository Server Executable
a = Analysis(['routes.py'],
             binaries=[],
             datas=[
                ('config.ini', '.'),
                ('config.ini.default', '.'),
                ('store', './store'),
                ('users.dat', '.'),
             ],
             hiddenimports=[],
             hookspath=[],
             hooksconfig={},
             runtime_hooks=[],
             excludes=[],
             win_no_prefer_redirects=False,
             win_private_assemblies=False,
             cipher=block_cipher,
             noarchive=False)
a_pyz = PYZ(a.pure, a.zipped_data,
             cipher=block_cipher)

a_exe = EXE(a_pyz,
          a.scripts,
          [],
          exclude_binaries=True,
          name='AAS_Repository_Server',
          debug=False,
          bootloader_ignore_signals=False,
          strip=False,
          upx=True,
          console=True,
          disable_windowed_traceback=False,
          target_arch=None,
          codesign_identity=None,
          entitlements_file=None, )

# CC Submodel Generator Executable
b = Analysis(
   ['aas_generator/cc_identifier_to_endpoint_address.py'],
   pathex=[],
   binaries=[],
   datas=[
      ('aas_generator/config.ini', 'aas_generator'),
      ('aas_generator/config.ini.default', 'aas_generator'),
   ],
   hiddenimports=[],
   hookspath=[],
   hooksconfig={},
   runtime_hooks=[],
   excludes=[],
   win_no_prefer_redirects=False,
   win_private_assemblies=False,
   cipher=block_cipher,
   noarchive=False,
)
b_pyz = PYZ(b.pure, b.zipped_data, cipher=block_cipher)

b_exe = EXE(
   b_pyz,
   b.scripts,
   [],
   exclude_binaries=True,
   name='Generate_CC_Submodels',
   debug=False,
   bootloader_ignore_signals=False,
   strip=False,
   upx=True,
   console=True,
   disable_windowed_traceback=False,
   argv_emulation=False,
   target_arch=None,
   codesign_identity=None,
   entitlements_file=None,
)

# Collect both executables in one folder
coll = COLLECT(a_exe,
               a.binaries,
               a.zipfiles,
               a.datas,
               b_exe,
               b.binaries,
               b.zipfiles,
               b.datas,
               strip=False,
               upx=True,
               upx_exclude=[],
               name='AAS_Repository_Server')
