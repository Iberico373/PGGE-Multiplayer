using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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
        
        //Used to write current settings for the camera into a text file in json format
        public static void SaveData()
        {
            string directory = Application.persistentDataPath + @"\DataFiles";
            string filename = Application.persistentDataPath + @"\DataFiles\gameconstants.txt";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }      

            try
            {
                using (StreamWriter str = new StreamWriter(filename))
                {
                    CameraConstants cons = new CameraConstants();
                    str.Write(JsonUtility.ToJson(cons));

                    Debug.Log("File saved succesfully!");
                }
            }

            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        //Used to read data from a text file then
        //set the properties of the "GameConstants" class
        public static void LoadData(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);

            try
            {            
                //Closes I/O stream if an error is encountered when calling OpenText()
                using (StreamReader reader = fi.OpenText()) 
                {
                    CameraConstants cons = new CameraConstants();
                    string json = reader.ReadToEnd();
                    cons = JsonUtility.FromJson<CameraConstants>(json);       
                    ProcessInputText(cons);
                }                
            }
            
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        //Processes input text to set GameConstants values
        private static void ProcessInputText(CameraConstants cons)
        {
            //A series of tries and catches in order to ensure that the
            //'GameConstants' variable are set correctly.
            //If there are any formatting errors while setting the variables (e.g., less than 3 values in the first line),
            //set variables to a set default value

            try
            {
                CameraAngleOffset = cons.mCameraAngleOffset;
            }

            catch (FormatException ex)
            {
                Debug.LogError(ex.ToString());
                Debug.Log("Setting to default value instead");
                CameraAngleOffset = new Vector3(10.0f, 0.0f, 0.0f);
            }

            Debug.Log("CameraAngleOffset: " + CameraAngleOffset);

            try
            {                
                CameraPositionOffset = cons.mCameraPositionOffset;
            }

            catch (FormatException ex)
            {
                Debug.LogError(ex.ToString());
                Debug.Log("Setting to default value instead");
                CameraPositionOffset = new Vector3(0f, 2.0f, - 4.0f);
            }            

            Debug.Log("CameraPositionOffset: " + CameraPositionOffset);

            try
            {
                Damping = cons.mDamping;
            }

            catch (FormatException ex)
            {
                Debug.LogError(ex.ToString());
                Debug.Log("Setting to default value instead");
                Damping = 100.0f;
            }
            Debug.Log("Damping: " + Damping);

            try
            {
                RotationSpeed = cons.mRotationSpeed;
            }

            catch (FormatException ex)
            {
                Debug.LogError(ex.ToString());
                Debug.Log("Setting to default value instead");
                RotationSpeed = 5.0f;
            }
            Debug.Log("RotationSpeed: " + RotationSpeed);

            try
            {
                MinPitch = cons.mMinPitch;
            }

            catch (FormatException ex)
            {
                Debug.LogError(ex.ToString());
                Debug.Log("Setting to default value instead");
                MinPitch = -30.0f;
            }
            Debug.Log("MinPitch: " + MinPitch);

            try
            {
                MaxPitch = cons.mMaxPitch;
            }

            catch (FormatException ex)
            {
                Debug.LogError(ex.ToString());
                Debug.Log("Setting to default value instead");
                MaxPitch = 30.0f;
            }
            Debug.Log("MaxPitch: " + MaxPitch);
        }
    }

    //Class meant to store 'GameConstant' values to be serialized into a JSON file 
    [Serializable]
    public class CameraConstants
    {
        public Vector3 mCameraAngleOffset = GameConstants.CameraAngleOffset;
        public Vector3 mCameraPositionOffset = GameConstants.CameraPositionOffset;
        public float mDamping = GameConstants.Damping;
        public float mRotationSpeed = GameConstants.RotationSpeed;
        public float mMinPitch = GameConstants.MinPitch;
        public float mMaxPitch = GameConstants.MaxPitch;
    }

    public static class PlayerConstants
    {
        public static LayerMask PlayerMask { get; set; }
    }    
}
