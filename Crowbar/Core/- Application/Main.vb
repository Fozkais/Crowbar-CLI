Imports System.IO

Module Main

   Public TheApp As App

    Public Function Main() As Integer
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf ResolveAssemblies

        TheApp = New App()
        TheApp.Init()

        Dim args As String() = Environment.GetCommandLineArgs()
        If args.Length < 2 Then
            Console.WriteLine("Usage: CrowbarCLI.exe <input_mdl> [output_folder]")
            Return 1
        End If

        Dim inputMdl As String = args(1)
        ' Définition du dossier de sortie (argument 2 ou dossier du MDL par défaut)
        Dim outputFolder As String = If(args.Length > 2, args(2), IO.Path.GetDirectoryName(inputMdl))

        ' --- CONFIGURATION OBLIGATOIRE DES SETTINGS ---
        TheApp.Settings.DecompileMdlPathFileName = inputMdl
        TheApp.Settings.DecompileMode = InputOptions.File ' Force le mode fichier [cite: 6]
        
        ' Forcer l'activation des sorties souhaitées 
        TheApp.Settings.DecompileQcFileIsChecked = True
        TheApp.Settings.DecompileReferenceMeshSmdFileIsChecked = True
        TheApp.Settings.DecompilePhysicsMeshSmdFileIsChecked = True
        TheApp.Settings.DecompileBoneAnimationSmdFilesIsChecked = True
        
        ' Paramètres de dossier de sortie
        TheApp.Settings.DecompileOutputFolderOption = DecompileOutputPathOptions.WorkFolder
        TheApp.Settings.DecompileOutputFullPath = outputFolder

        ' --- INITIALISATION DU DÉCOMPILATEUR ---
        Dim decompiler As New Decompiler()
        
        ' IMPORTANT : Initialisation manuelle du chemin de sortie [cite: 3]
        ' Dans le mode normal, c'est fait par DoWork, ici on doit le faire à la main
        decompiler.theOutputPath = outputFolder

        Try
            Console.WriteLine("Target MDL: " & inputMdl)
            Console.WriteLine("Output Folder: " & outputFolder)

            ' Lancement de la décompilation [cite: 6]
            Dim result As AppEnums.StatusMessage = decompiler.Decompile()
            
            If result = StatusMessage.Success Then
                Console.WriteLine(">>> Decompilation Successful!")
                Return 0
            Else
                Console.WriteLine(">>> Failed with status: " & result.ToString())
                Return 1
            End If
        Catch ex As Exception
            Console.WriteLine("CRITICAL ERROR: " & ex.Message)
            Return 1
        Finally
            TheApp.Dispose()
        End Try
    End Function

    ' Gardez la fonction ResolveAssemblies telle quelle
    Private Function ResolveAssemblies(sender As Object, e As System.ResolveEventArgs) As Reflection.Assembly
        Dim desiredAssembly As Reflection.AssemblyName = New Reflection.AssemblyName(e.Name)
        If desiredAssembly.Name = "Steamworks.NET" Then
            Return Reflection.Assembly.Load(My.Resources.Steamworks_NET)
        Else
            Return Nothing
        End If
    End Function

End Module
