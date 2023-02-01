using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PGGE
{
    public static class GameConstants
    {
        public static Vector3 CameraAngleOffset { get; set; }
        public static Vector3 CameraPositionOffset { get; set; }
        public static float Damping { get; set; }
        public static float RotationSpeed { get; set; }
        public static float MinPitch { get; set; }
        public static float MaxPitch { get; set; }

        //Used to read data from a text file then
        //set the properties of the "GameConstants" class
        public static void LoadData(string filepath)
        {
            try
            {
                FileInfo fi = new FileInfo(filepath);

                //Closes I/O stream if file is null/incompatible
                using (StreamReader reader = fi.OpenText()) 
                {
                    //Open the file in the file path to read
                    //using the OpenRead() function
                    FileStream fs = File.OpenRead(filepath);
                    //Read characters from incoming byte stream
                    StreamReader sr = new StreamReader(fs);

                    //List of string to store lines of text from the file
                    //that's being read
                    List<string> text = new List<string>();

                    while (!reader.EndOfStream)
                    {
                        text.Add(reader.ReadLine());
                    }

                    ProcessInputText(text);
                }                
            }
            
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        //Processes input text to set GameConstants values
        private static void ProcessInputText(List<string> text)
        {
            //Splits first line of gameconstants.txt into seperate strings 
            //after every space
            string[] str_cameraAngleOffset = text[0].Split(" ");
            //Set camera angle offset to the first three strings in
            //the 'str_cameraAngleOffset' string array
            CameraAngleOffset = new Vector3(float.Parse(str_cameraAngleOffset[0]),
                                float.Parse(str_cameraAngleOffset[1]),
                                float.Parse(str_cameraAngleOffset[2]));

            Debug.Log("CameraAngleOffset: " + CameraAngleOffset);

            string[] str_cameraPositionOffset = text[1].Split(" ");
            CameraPositionOffset = new Vector3(float.Parse(str_cameraPositionOffset[0]),
                                float.Parse(str_cameraPositionOffset[1]),
                                float.Parse(str_cameraPositionOffset[2]));

            Debug.Log("CameraPositionOffset: " + CameraPositionOffset);

            Damping = float.Parse(text[2]);
            Debug.Log("Damping: " + Damping);

            RotationSpeed = float.Parse(text[3]);
            Debug.Log("RotationSpeed: " + RotationSpeed);

            MinPitch = float.Parse(text[4]);
            Debug.Log("MinPitch: " + MinPitch);

            MaxPitch = float.Parse(text[5]);
            Debug.Log("MaxPitch: " + MaxPitch);
        }
    }
    public static class PlayerConstants
    {
        public static LayerMask PlayerMask { get; set; }
    }    
}
