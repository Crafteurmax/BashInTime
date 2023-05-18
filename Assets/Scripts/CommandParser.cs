using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class CommandParser : MonoBehaviour
{
    //Special selection Command
    [SerializeField] private string specialCommand = "echo Bonjour";
    [SerializeField] [TextArea] private string specialOutput = "Bonjour\nBonjour Chell, je dois rester discret, retrouve mes infos avec \"cat info.txt\"";

    [SerializeField] private string[] forbidenDir;

    //Racine reelle des commandes
    private string root = "BashWork/root";
    //CD virtuel
    public string currentDirectory = "/";

    private string authorizedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 \n\r\t/\\.-_<>*|\"\'[](){}"; //Temporaire, sera fait aussi dans la saisie des caractères

    //Type de commande(voir ci-dessous)
    public enum CommandType : int
    {
        Error = -1,
        Direct = 0,
        File = 1,
        Interpreted = 2
    }

    [SerializeField] private ChefDorchestre chef;
    
    [SerializeField] private string[] fileCommands = {"ls", "rm", "grep", "mkdir", "cat", "touch", "cp", "find", "mv", "head", "tail", "wc", "tar"}; //Contient des arguments principaux qui sont des fichiers
    [SerializeField] private string[] directCommands = { "echo"}; //Contient des arguments principaux qui sont des donnees directes
    [SerializeField] private string[] interpretedCommands = { "cd", "pwd", "unlock", "man" }; //Commandes interpretees directement


    //Variable pour la commande finale, pratique avec les tubes
    private string rawCommand = "";
    private Action<string, string, string> finalCallback;
    private string rawUserCommand;

    //Lien avec l'executeur de commandes
    private CommandExecuter executer;
    private Typing keyboard;
    private void Awake()
    {
        Debug.Log(root);
        executer = GetComponent<CommandExecuter>();
        keyboard = GetComponent<Typing>();

        rawCommand = "";
        finalCallback = ShowReturnValue;
        rawUserCommand = "";
    }

    
    //Fonction Callback qui est appelee et recoit le message a afficher sur la console
    public void ShowReturnValue(string textLog, string errorLog, string command)
    {
        DetectPalais(textLog);

        if (errorLog.Trim() != "") keyboard.PrintOutput("<color=red>" + errorLog + "</color>");
        else keyboard.PrintOutput("<color=#00FFFF>" + textLog + "</color>");
    }

    

    //Fonction Principale d'execution d'une commande
    public void ExecuteCommand(string command)
    {
        string[] lines = command.Split("|");
        foreach (string line in lines)
        {
            if(!PreparePart(line, lines.Length)) return;
        }

        System.Text.StringBuilder lineBuilder = new System.Text.StringBuilder(rawCommand);
        lineBuilder[rawCommand.LastIndexOf('|')] = ' ';
        rawCommand = lineBuilder.ToString(0, rawCommand.Length);

        System.Text.StringBuilder userLineBuilder = new System.Text.StringBuilder(rawUserCommand);
        userLineBuilder[rawUserCommand.LastIndexOf('|')] = ' ';
        rawUserCommand = userLineBuilder.ToString(0, rawUserCommand.Length);

        RawExecute();

        rawCommand = "";
        finalCallback = ShowReturnValue;
        rawUserCommand = "";
    }


    //Prepare l'execution d'une partie de la commande(chaque commande entre les tubes)
    public bool PreparePart(string line, int ncommands)
    {
        line = line.Trim();

        //Exception
        if (line.Equals(specialCommand))
        {
            ShowReturnValue(specialOutput, "", specialCommand);
            return false;
        }

        //On supprime les caracteres illegaux
        foreach(char c in line)
        {
            if (!authorizedCharacters.Contains(c))
            {
                ShowReturnValue("", "Illegal Character : '" + c + "'\n", line);
                return false;
            }
        }


        //On recupere la commande et on appelle des fonctions differentes en fonction de celle-ci
        string[] words = PrepareOutputInputCommand(line).Split(" ");

        if (words.Length <= 0) ShowReturnValue("", "Please Input something\n", line);

        string command = words[0].Trim();
        CommandType type = GetCommandType(command);

        switch (type)
        {
            case CommandType.Error:
                ShowReturnValue("", "bash: "+command+": command not found\n", line);
                return false;
            case CommandType.Direct:
                DirectPrepare(words, ShowReturnValue, line);
                break;
            case CommandType.File:
                SafePrepare(words, ShowReturnValue, line);
                break;
            case CommandType.Interpreted:
                Interprete(command, words, line, ncommands);
                break;
        }

        return true;
    }

    //On rajoute des espaces pour permettre au programme de prendre en compte entree/sortie comme des fichiers
    private string PrepareOutputInputCommand(string command)
    {
        return command.Replace(">", " > ").Replace(">  >", ">>").Replace("<", " < ");
    }


    //Fonction qui recupere le type de commande
    public CommandType GetCommandType(string command)
    {
        foreach(string com in directCommands)
        {
            if (com.Equals(command)) return CommandType.Direct;
        }

        foreach (string com in fileCommands)
        {
            if (com.Equals(command)) return CommandType.File;
        }

        foreach (string com in interpretedCommands)
        {
            if (com.Equals(command)) return CommandType.Interpreted;
        }

        return CommandType.Error;
    }

    //Fonction qui recupere un chemin absolu virtuel (et relatif reel) a partir d'un chemin relatif virtuel (supprime les ..)
    private string GetAbsoluteVirtualPath(string path)
    {
        path = path.Trim();

        bool isAbsolutePath = path[0] == '/';

        int maxCountBack = -1;

        foreach (char character in currentDirectory)
        {
            if (character == '/') maxCountBack++;
        }

        if (isAbsolutePath)
        {

            for (int i = 0; i < maxCountBack; i++)
            {
                path = "../" + path;
            }
        }

        Stack<string> pathStack = new Stack<string>();
        string[] elements = path.Replace("\\","/").Split("/");

        //On utilise une pile pour resoudre les ..
        int backCount = 0;

        foreach (string element in elements)
        {
            switch (element)
            {
                case "":
                    break;
                case "..":
                    if (pathStack.Count > backCount) pathStack.Pop();
                    else
                    {
                        if(backCount < maxCountBack)
                        {
                            backCount++;
                            pathStack.Push("..");
                        }
                    }
                    break;
                case ".":
                    break;
                default:
                    pathStack.Push(element);
                    break;
            }
            
        }

        //On recontruit le chemin a partir de la pile

        string new_path = "";

        while(pathStack.Count > 0)
        {
            new_path = "/" + pathStack.Pop() + new_path;
        }

        

        new_path = "." + new_path;

        if (new_path == "." || new_path == "./" || new_path == "/") return "";

        if (new_path == "/." || new_path == "/./") return "/";

        return new_path.Substring(2,new_path.Length-2); //To avoid non file arguments to be invalid
    }

    //Permet de savoir si une fonction est une option du type -blabla ou un argument principal
    private bool IsOption(string word)
    {
        if (word.Length == 0) return true;

        if (word.Trim() == ">" || word.Trim() == ">>" || word.Trim() == "<") return true; //On detecte si c'est pas un symbole entree sortie

        if (word[0] == '-')
        {
            for (int j = 0; j < word.Length; j++)
            {
                if (word[j] != '-')
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void SafePrepare(string[] arguments, Action<string, string, string> callback, string userCommand)
    {

        //Detection d'arguments, safe car chemin relatifs transformés en chemins absolus virtuels
        for(int i = 1; i < arguments.Length; i++)
        {
            arguments[i] = IsOption(arguments[i]) ? arguments[i] : GetAbsoluteVirtualPath(arguments[i]);
        }

        string newCommand = "";

        foreach(string argument in arguments){
            newCommand += " " + argument;
        }

        RawPrepare(newCommand, callback, userCommand);
    }

    //Fonction intermédiaire pour prendre en charge l'entree/sortie avec la securite sur les fichiers de sortie
    private void DirectPrepare(string[] arguments, Action<string, string, string> callback, string userCommand)
    {
        int i = 0;
        while (i < arguments.Length && arguments[i] != "<" && arguments[i] != ">" && arguments[i] != ">>") i++;

        for (i++; i < arguments.Length; i++)
        {
            if(arguments[i] != "<" && arguments[i] != ">" && arguments[i] != ">>" && arguments[i].Trim() != "")
            {
                arguments[i] = GetAbsoluteVirtualPath(arguments[i].Trim());
            }
        }

        string finalCommand = "";

        for(int j = 0; j < arguments.Length; j++)
        {
            finalCommand += arguments[j] + " ";
        }

        RawPrepare(finalCommand, callback, userCommand);
    }



    //Rajoute directement la partie de la commande
    private void RawPrepare(string line, Action<string, string, string> callback, string userCommand)
    {
        rawCommand += line + " | ";
        rawUserCommand += userCommand + " | ";
        finalCallback = callback;

        return;
    }

    //Execute la commande finale
    private void RawExecute()
    {
        StartCoroutine(executer.Execute(rawCommand, currentDirectory, finalCallback, rawUserCommand));
    }

    //Callback de la console pour CD, permet de savoir si le dossier existe
    public void CdCallback(string log, string errorLog, string command)
    {
        string oldCurrentDirectory = currentDirectory;


        if (errorLog.Trim() != "")
        {
            ShowReturnValue(log, errorLog, command);
            return;
        }

        string[] arguments = command.Trim().Split(" ");

        //Si il manque des arguments, on retourne a la racine
        if(arguments.Length <= 1)
        {
            currentDirectory = "/";
        }
        else
        {
            //On recupere l'argument correspondant au fichier, et on le met sous la forme d'un chemin virtuel absolu.
            int fileIndex = 1;
            while (fileIndex < arguments.Length && IsOption(arguments[fileIndex])) fileIndex++;
            if(arguments[fileIndex].Trim()[0] == '/')
            {
                currentDirectory = ("/" + GetAbsoluteVirtualPath("/" + arguments[fileIndex])).Replace("/.", "/").Replace("//", "/").Replace("./", "").Replace("./", "") +"/";
            }
            else currentDirectory = ("/" + GetAbsoluteVirtualPath(currentDirectory + "/" + arguments[fileIndex])).Replace("/.", "/").Replace("//", "/").Replace("./", "").Replace("./", "") + "/";

            if (currentDirectory == "//" || currentDirectory == "/./") currentDirectory = "/";
        }

        foreach(string forbiden in forbidenDir)
        {
            if (currentDirectory.StartsWith("/" + forbiden))
            {
                ShowReturnValue("", currentDirectory + " : Access Forbidden", command);
                currentDirectory = oldCurrentDirectory;
                return;
            }
        }
        

        ShowReturnValue(log, errorLog, command);
    }

    //Switch d'interpretation des commandes
    private void Interprete(string command, string[] arguments, string userLine, int ncommand)
    {
        switch (command)
        {
            case "pwd":
                arguments[0] = "echo " + currentDirectory; //SOLUTION DE GENIE POUR LES PIPES!!!!
                DirectPrepare(arguments, ShowReturnValue, command);
                break;

            case "cd":
                if(ncommand != 1)
                {
                    ShowReturnValue("", "Using cd with pipes is not allowed\n", "cd"); //Bloquer le cd avec le pipes aucun interet et bugs possibles
                }
                SafePrepare(arguments, CdCallback, userLine);
                break;

            case "unlock":
                unlock();
                arguments[0] = "echo ";
                DirectPrepare(arguments, ShowReturnValue, command);
                break;

            case "man":
                if (arguments.Length == 2)
                {
                    arguments[0] = "cat ";
                    arguments[1] = "/usr/share/man/" + arguments[1];
                    command = arguments[0] + arguments[1];
                    SafePrepare(arguments, ShowReturnValue, command);
                }
                else
                {
                    ShowReturnValue("", "man needs only 1 argument", command);
                }
                break;
        }
    }


    [SerializeField] string forlderAdresse = "testCopyPaste/";

    bool IsKeyPlaced()
    {
        try
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader Green = new StreamReader(forlderAdresse + "GreenDoor/GreenKey.txt");
            string GreenPassword = "Hyjnc * FswTshvLDbGNG8y6J5yZgBFDbbASqwB8xZwj4r ? GA = 4L4!rj3MD & R *=%% qfmCE!Bky2Rv7gYD6RNad & jSD88jW = EWzAQW * WbdN$?k7hejDrUhDj6kG_NZ7wTjaFyN6DN + PRZBgcQRQ ^ ygCZ2m & 8jLF + t8 +#C-DJC*&JRmS2mJ&#z=2wnNQ_KVR_NGFq^vM-&Eh2rXwUeyN*rqvcZED2J2V@Yj-&Bg4^93S+^f*9=p%=Gp$Z_K%Dw?8Ra%tBm!2Df+K#B_QxB7wGhwvhvRp*CK#z@s37S^^S%5ntKMd#NGw2^wmJ7*xJcY^8%TbNbcw??AWm4U4QtnSwbew9w8c&F6Un=8M8Nt_apd+A*jUQZr8&u!zsD34en!Tg2Y9FA4WrDhV@?Mp5y-xAvrBg$7Y!KVCJ@T%k-@P!_WyQ#q++CqMdF*Rtr6PPD^wAhbjRQMDg--7a?kHENV^-c4GE-CFU4wnu8Hv!!uEVsWG_DfmYy-y_yY9Kh2QxSHBtaNt7*#FrjR59?FaFgr6W7S4q3dmKaAHEvjExU?%8cJc^b!63EB65!VV3j*ft*@RVNN&FD$NpfjQH9XKU!gZykMY%S$eL^4Fm3y-fuLexYZ6WQ*ssm?rGJfVjvQ?e9B$!@#?LZgFu?KpL4f4Gk7ATaCZ=T&TfApsaN=HP&XdW2BSDn6Z9aQME33*5Gp%-9-?uRda!43UbwxU&Pat+C-ea&2Bc$Yq4B$DT^&$Bt9@KxEjwR-Qfe=zHmLpkL-!LQ!DteT#kk=t9V8j^tx=x!vB8@JKXXdSMXj+_=mxR2gpL%PRUP+#z=P*$ufNfmDTbn$VVu73L$NQmD_Vs*f*-@x=gTWQ*ghjN=&H=JD$9S4UQ5UDzKeav4ZfuFSxb$3Fp&-@K#J^%*WT&t@M&R_zVAMKkB5SdPQDEJM-QmQ4+AYekVNAmW4D%Rw6mbX*VQeUdZM=Q-9Tc$#yU@xa+7LpfZ!Lv?zFex2t7m$b6sG^c2D7n43g7KTVmMYhxX^$Y3GytnstdzT2@EEeB$D^MDVMm8HF=q@n+CF*aD-dTp4m?-k48Grtp!@-&RW6Fz6+Tunb!w5fg7u5FT9vc6N5QmXc@2#2&Xjv@P&?SxRw#D9Z-DQTCgw_-7CL!E_fkKXm6J!QufV276+DM5aaF@z7z$Y@?^R8=mnJ!WvZ*N6VW4rEHbw%^$Y55+-m5_&+3@PPaAhQt8sb48x^EHuWVhtPBYf+BJ5G_5$ZYHP&X?BhnE^DJd$K*7CPdmHFrMXt*-P5*54WGxzMW6YDYTH-RFm$8YwLV_mS6dEzVU-_BMc36gp2zdTbne*b6n3HunYJDnVP*D=SRG%L2?*Jy*e4hPRDh$EBuJmU_HqmrZrGLR#Ae*P7%j4jH9n#_M5m5W%Wy$=9m%Lx%JELWv8BL$bfjJreQSZwE5BKmjdq6bSJ6v^^=D$2VR?zw9A^y=kM=Hv?5*_%*LqwcQ+Hrq^aB$fSFc7f?y5p$yJ+Q^=Src-jGCuhPkw*a%cx4L_#gJGt2z?LVSPrF!@c79Z!AUxUCEFrx@*udes$v4%*2vc*u*_KR^wg-hH_5rJ9&*K@qDYKrZSNNsuuD5WEm-?2VN4w%q!UMDX_Pv@!cj+Lzyst*E6rBL+y+4vEWrUF#*#kxDs7#gJ+jbu5HEgruHvaV#7BYT@ZQwKFH_&fy_*EC_jNeqSJHKd+fu4@a*3h4XkX^Cjwy5x%78K*r3XRzMMvKH^%mAk8Eg9*63j?_5gH?SH9_?6av5#pntvd4xZCQx9AANUFf8RjacyEDA8+bNq**FDycRLW!NhDY*@akcTk8r2dVG-wWSd&NrB#rBr4+265#&?kg_dgV8@njBY+?T%fsH4cGu$tk6VKeh-@#nE%Zeyn8AN@gnnXnu2X4m8Q4QRJNjWnd#SR?EBz%q@rwK778ZTJa-V#JMTfpDu9ejaLyr9FRNuy7a35xuzpXt=h#M9W9t&fS_*CE_=B9B8puBA7S@jfC#Wr!5n3sts$c$Ev%vDxW7D8u@Kv@Tcwz#MwcCXvY7Ae4g4hyPL2cuvdAvr8g#RV";
            string GreenLine = Green.ReadLine();
            if (File.Exists(forlderAdresse + "GreenDoor/BlueKey.txt") || File.Exists(forlderAdresse + "GreenDoor/RedKey.txt"))
            {
                return false;
            }

            StreamReader Blue = new StreamReader(forlderAdresse + "BlueDoor/BlueKey.txt");
            string BluePassword = "^8+nBa7UBDqFbZzG7!q?VSHVB#B4EKVAyzJS?YH#@7?pv968+UZ?xDJ+3bG&!wKh7zws^zBCQnt_tcqqDP-=g#tFTSDprkBRy2bL%uZHd7pV^Wn+Z%Z*7v&fh$^r?2Mvnmt23b$$+f-?x-PRJSf6x*Qz46hVBH3cS6p^24jsa_dxHC4H=#xbzLU@^%ssCNpm9FWv-&3u%=hK-ch#vMqv4VfrQgbGhSRTEahS!w#VKdZ@Hz&^mK8+x^3H@^6P+8QBTZ8nLSqK+Bdc!$pt_j=kH_ZzZULV!?wh8_#rMu3=*B#zccsHJrK!-CEKyv3cxMc8UCF&rx-aKgWKxrMTAzjw2F6kGtz!zPbfEtrk4WCBn74V_Q&^hSv^YJ2D^qKkZyxtJ+tvkjW#Cd&8rnfRLatRw$A4HJv753^3P8gHRdA92$k4uFRV4Y+?^@^QD*FsvdFD@YxJBDmwwC2p-#Gn^YZv*cg5&uqwy+QFGPUVvtzwRxM+Jm4&yv=7Wa5=@X7vC$CHhXb=Y8DUz+b6xADQD&@NNJ!7L$G86JudRBn7LzAECX%8*E-KXvwp87kB^w!xL43AW#v++KFTXKPB*nmnbmY8@P8!92!J*W=Sb8^EU!ZRTn=22h^+^-GUsss^upX?@G4+XnM6GEwNnDKQBr^^Ced+9MTv&bn8&wnFqSe*xC%+^mQ_9#fZVWh3rFRGTvfmkc9LyQjFTs?av=%J+wCt6gCs3c#4?QC6=?AAbYh5bHZ#h_FVnx7*g4=@@Fe3rPG$m&GJ_9zxLbmtCg9L?-Y$RPRnJ2xx%gf8pAD%d#ebnuxaxPM-jY9_DEwTc8e2&DKnVZLeVHt8aD-$vTCTvXnZz=PV*HpwYjgX68jQYetY9bYyaKX#%+by_-mPTu&YNXv?4L2_d&pfE_WAmp4xp!zKDwgBjX%$#m6TFrUnV+yc-kcfdQx*@ySSWC6Jp9N?yDnvW85y%EtTGVUBp$v^vtEELeU9EhV4+KCE+Dn@k2th8_hW^v@#&!j=JgsS#=^J&mNTE#Sakqzfcr6%B+zvJM=GR!MT#J&W*nvZbT*7Td!FDd$8JmXN%GLf_EXdc?Bxk^T5RN?CFvbZuEYymV=@z%tR47nU8ebW&SAmL@CxF?Jhn7Z4jNDM%9@V%_VhuGS2+Q9$_B5%CgT^-bccQy@%_%W*j3hSgX+MaRXmpEJ$zjHM?b-SKwRyrER#Y6_$mcn-ju5E4PJ4VLrx5q*cy2hHkY_NDBwm+LpxG-%RxFU#H$6+SSnv#vR+dqpunQkaVM9aYAUDKu%2W6vS_VJ2QH?-JPqKww_X!2Nf?3-y4vq&sFg@HNEs4Eu9m3E7LgmMn*DzXNDtQ%2QjArp4tUF&YwG?r+X@F^7&_6H=ptkm&K5Vt@peZ%SVHzFzj$Exnqe&!bA==H=*?KxHRdb93p#QMPcg8uBwCp#zuj9y9C&X2bF-@Uj4Ua@q_w8LE#Fp+Gf+73DDDw+#GPwfx_S5Z+s=CJQ*LB*6?kMs2qDD4t$HukmSuM_9P&#TaZAad5%nQ!Gvg-Wfm86jegdXzzxfF+nYBhfAYEsAfCbEeCyuk984CsFX^Zk?LfWzESTFC=z@Y2*@NWgPtc*Wd7_RANtKavW_td!sh5Z5ga&nt#FB*YRHa#ZTFC84!Ax&%UWn85DJM-S72dEM9rX*!^n=Ae*3@tH#aBrq*mxHhTSQcy7+tpv!^zz8FATeV9GC&ZgCaXy@hbB+B8YLk!ga$xgJL6c%Sd-A*Tu^2H872cy5!2MgpZ6=3CwAwG3pT42yWevF6UJ8$-=k+Wc=ua?Jcs^HJ8dKHCk-9ANTZ3=75_vF*zf!Ns5hcc5C5&BP%bu4dn2!vL#-%NWk%XqDQW^UQg!%@X+_Kj-9Yav_cznPHCMJM#5*B7ZN78TEUEFC7pt4E-X=@G+A+=%N6LGZp5MUv43@aY4rpebKzwQMHLCxxZk-qV^u?SVZzK7+RLN94zwGrEUuVHLNH8CeN?e+t-x7nADtUjjQ*Ly%X%qkvR_jSvE2evS7F3ePM%Sbp^Fc_%3NL%CCuRA#Dvm^5MGWWgVg6M@";
            string BlueLine = Blue.ReadLine();
            if (File.Exists(forlderAdresse + "BlueDoor/GreenKey.txt") || File.Exists(forlderAdresse + "BlueDoor/RedKey.txt"))
            {
                return false;
            }

            StreamReader Red = new StreamReader(forlderAdresse + "RedDoor/RedKey.txt");
            string RedPassword = "xxVJARPN9CVeVgcrn9#RqGuVXD!Qsv+wnmSncc*??@FQA6L^@$JQY7hKmnxK2MMAeRm42wxZ3YHFG9h2!5A&AxY&_!y!-27p#fnuNGNU&S=TNMPH&w4+@M&%UQcCubFbK@uReumKVwcPxv+aEQ6Q*p6U&npTaYCXLTa*5R=LAuDFF4yBPzZETsr$+5xu2eng@P%zz$gT^PD3k-HeY5^y4-*p=-M^cx#+S2UaX2^P3d_QvTb6re9VN3w%DT*w9RV#DRngs&Fc$5gss_=r5L7^XV^E+nAvAJ8P7XZ6=vSaQSqMrqhm!hPbqa=*hbt_eBZPRHKSvRA&*QabP$-KCGZwAz7YGn4tPZqZZE7Ds99ZU*76WuRVGj!Cpxr!a!YeR*RUsAUw^V4K*VAddM*QVtrLFyP=jb^qrGEcz-zZp?zX?Ht@pRbCbXEKQXK#d_?&@VBnrNTKeD9^EJ#dhuZvQL&tZjpNBmw2fu4uN-zZSj^b6g?*%UZ-h=9FW_xgX@#^2ac!=Rr2NYx@K^%yZ&jG@=CvG+eK7UEhdnXjr?KvmNZDaZdCAD-D7zH2+pt_^Qa=Z?GJqwbUr@cJuEnNEy$5LVhA$cwYC65jhxuzwmDMm$gb5NBRyK!q9?p+_hxfbTvATt#tej=sg#ySjRH$DF=6X!6jwNynMJ9#G2h9&+-3H$d3=hhDp8E?Nr2xZ6AFSg%uzN!RCDU=kwFCdhwsDSBRF#h=B*zsAA8Dc!9k6=tM#7-KK-Su9h&6GSZ2LWRGbN+wQ8CJF-a?x&%Pbntqs7J9TKJ+E_sb4C#rt_Dyc4B%%JHe?p*eCEgqwAhS#cuaEspVcfa2Hq9snUDVs?GFn6sTCc-xeN-Y-4W3YZSCsubn#cBxS!NE#nz7tX7fyJnpEZvNRufr@6uCJme6mT@md232S@fTAux+m^=yP^XxLNx$5v6d8e5YKP%PnKCAMX$e9T#ZCHj7568m+Rd-zG__ZrzCjdVa&$JZ-C!_^t5V6ERK@J?g5WnCzk2m!zdH#M_t8%K+t84xRXV#VDDS$Ax4DRjX%GYZBA!3zg!L-m^$qfeZfsRhk&_Dw#HPtSbbtF&9B$3HWfjWaF$@efUwXKNqzNysQpBwxzLM7nAvkn5euJFKEG=sT8Qp64M4@fk-tm?4-5p&hznm?HH4%$hQauG-QYcky9_BYMccCPtsMZaza#VgYBS2ffTP5&$J-k%fqVPf+r@MDMaW2WV_8Y+krF2bMZTYxr!L^2Prpy&mtP2pt98t+#gcjhVKJ-k-BA3x7xutam_VQT84jQTMbPaJzJBzkdp?hhd@7u+&Bv-MY^=Qfw9nfqLG?9PxYr7tg@s+PStwJ*ZbTs^=TaAgUbu@?ZyB2YC4PZqnMUuMw6kzGjujM5Y$?dy5s?e8CaELguqqSXsL?WYsq#XEQ$zvLh+U*mv3fDR$LZCPgdgGU&VJ?S-zH2c%zKG7qa6ZwW&tcvWeVBMf#KQCfL?UZC2Q+x!vvDcnTaud2*QYkQhz7sqXgKecdRjU*pa-Fs+@Xs44q8wdLHY*!Lh#e25hs7?T%QKTx_tyQEz4NX=3^4B4+E$MJrvN@y_s2x5Da6A3XVpRS24b7Tz-nc&!bRSJ$S9GtFMMmUU?us2^cSW*Mg-ABB=N6!-@rwsK9=rvfnpE!JTn7W5-f?%8#zZyqh%S6ZtnvW4J$hJHh$b-gAY?@nZ$ZT2Pg_MX73DkwKf6WT8%Fwmh?k=fbeJad7ThJNNb6usa!h5ZK+^jWS&6Ec8w+R4kW+_Ss@aFWgUpv4yd-Re^T2Y4#y$tcguEFu^D&w#NZ3D#-#E^qcB+x=pu?a^9u=nqcP^bM23B*ek5pQUBB6#&%dvv5M9x#r9zu&C8Fna%%wYQ+gMx9PQ6pnqG%SD_EY9T=sY8d@@&fQ7Pkx?#6V#QcCXB9fKh3?7Wwngfu2NDYQaqkBr5n^EEDxPhc_LFG?UhTJt%NKfk%_VBhGhXPW&zFwL82mMkTT=56=5+?yWJWRxuB7sb8RKLwZ_27v^7-7F-VWrmq?LAUUf-HckCj8$WmCpxgnEC=2BhrhMbEvJ$";
            string RedLine = Red.ReadLine();
            if (File.Exists(forlderAdresse + "RedDoor/BlueKey.txt") || File.Exists(forlderAdresse + "RedDoor/BlueKey.txt"))
            {
                return false;
            }

            StreamReader WhiteRed = new StreamReader(forlderAdresse + "WhiteDoor/RedKey.txt");
            string WhiteRedLine = WhiteRed.ReadLine();

            StreamReader WhiteBlue = new StreamReader(forlderAdresse + "WhiteDoor/BlueKey.txt");
            string WhiteBlueLine = WhiteBlue.ReadLine();

            StreamReader WhiteGreen = new StreamReader(forlderAdresse + "WhiteDoor/GreenKey.txt");
            string WhiteGreenLine = WhiteGreen.ReadLine();

            if (RedLine == RedPassword && GreenLine == GreenPassword && BlueLine == BluePassword && WhiteGreenLine == GreenPassword && WhiteBlueLine == BluePassword && WhiteRedLine == RedLine)
            {
                return true;
            }
        }
        catch (System.Exception e)
        {
            //Debug.Log(e);
        }

        return false;
    }

    void unlock()
    {
        //Debug.Log(IsKeyPlaced());
        Directory.CreateDirectory(forlderAdresse + "/corridor");

        using (StreamWriter sw = File.CreateText(forlderAdresse + "/corridor/code.txt"))
        {
            sw.WriteLine("32XXXX");
        }
    }

    //Pour debugger depuis l'interface Unity (Code debug)

    public string testline; //DEBUG
    public void testcommand() //DEBUG
    {
        ExecuteCommand(testline);
    }

    /*
    [CustomEditor(typeof(CommandParser))]
    public class testInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CommandParser parser = (CommandParser)target;
            if(GUILayout.Button("test command"))
            {
                parser.testcommand();
            }
        }
    }
    */


    public void DetectPalais(string detectString)
    {
        if (detectString.Contains("Voici une liste de commandes pour m'aider à me retrouver"))
        {
            PalaisMental.current.AddMemory(3);
        }
        else if (detectString.Contains("a clé blue pour la porte blue"))
        {
            PalaisMental.current.AddMemory(4);
        }
        else if (detectString.Contains("je suis chargé de fouiller une liste de dossiers à sa recherche"))
        {
            PalaisMental.current.AddMemory(5);
        }
        else if (detectString.Contains("j'avais noté le code au début du chapitre qui s'appelait"))
        {
            PalaisMental.current.AddMemory(6);
        }
        else if (detectString.Length < 20 && detectString.Contains("32XXXX"))
        {
            PalaisMental.current.AddMemory(7);
        }
        else if (detectString.Length < 20 && detectString.Contains("XX76XX"))
        {
            PalaisMental.current.AddMemory(8);
        }
        else if (detectString.Length < 20 && detectString.Contains("XXXX56"))
        {
            PalaisMental.current.AddMemory(9);
        }
    }
}
