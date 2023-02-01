using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGGE;
using System.IO;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public CharacterController mCharacterController;
    public Animator mAnimator;

    public float mWalkSpeed = 1.5f;
    public float mRotationSpeed = 50.0f;
    public bool mFollowCameraForward = false;
    public float mTurnRate = 10.0f;

#if UNITY_ANDROID
    public FixedJoystick mJoystick;
#endif

    private float hInput;
    private float vInput;
    private float speed;
    private bool jump = false;
    private bool crouch = false;
    public float mGravity = -30.0f;
    public float mJumpHeight = 1.0f;

    private Vector3 mVelocity = new Vector3(0.0f, 0.0f, 0.0f);

    void Start()
    {
        mCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
    }

    public void HandleInputs()
    {
        // We shall handle our inputs here.
    #if UNITY_STANDALONE
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
    #endif

    #if UNITY_ANDROID
        hInput = 2.0f * mJoystick.Horizontal;
        vInput = 2.0f * mJoystick.Vertical;
    #endif

        speed = mWalkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = mWalkSpeed * 2.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jump = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            crouch = !crouch;
            Crouch();
        }

        if (Input.GetKey(KeyCode.P)) 
        {
            SaveData();
        }

        if (Input.GetKey(KeyCode.L))
        {
            LoadData(@".\gamedata.txt");
        }
    }

    private Vector3 moveDirection = Vector3.zero;
    public void Move()
    {
        if (crouch) return;

        // We shall apply movement to the game object here.
        if (mAnimator == null) return;
        if (mFollowCameraForward)
        {
            // rotate Player towards the camera forward.
            Vector3 eu = Camera.main.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0.0f, eu.y, 0.0f),
                mTurnRate * Time.deltaTime);
        }
        else
        {
            transform.Rotate(0.0f, hInput * mRotationSpeed * Time.deltaTime, 0.0f);
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
        forward.y = 0.0f;

        moveDirection.y -= mGravity * Time.deltaTime;

        mCharacterController.Move(forward * vInput * speed * Time.deltaTime + moveDirection * Time.deltaTime);
        mAnimator.SetFloat("PosX", 0);
        mAnimator.SetFloat("PosZ", vInput * speed / (2.0f * mWalkSpeed));

        if(jump)
        {
            Jump();
            jump = false;
        }
    }

    void Jump()
    {
        mAnimator.SetTrigger("Jump");
        mVelocity.y += Mathf.Sqrt(mJumpHeight * -2f * mGravity);
    }

    private Vector3 HalfHeight;
    private Vector3 tempHeight;
    void Crouch()
    {
        mAnimator.SetBool("Crouch", crouch);
        if(crouch)
        {
            tempHeight = GameConstants.CameraPositionOffset;
            HalfHeight = tempHeight;
            HalfHeight.y *= 0.5f;
            GameConstants.CameraPositionOffset = HalfHeight;
        }
        else
        {
            GameConstants.CameraPositionOffset = tempHeight;
        }
    }

    //Saves player position and rotation into a txt file
    void SaveData()
    {
        string filename = @".\gamedata.txt";

        try
        {
            using (StreamWriter str = new StreamWriter(filename))
            {
                str.WriteLine(transform.position.ToString());
                str.WriteLine(transform.rotation.ToString());

                Debug.Log("File saved succesfully!");
            }
        }

        catch (IOException ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    void LoadData(string filepath)
    {
        FileInfo fi = new FileInfo(filepath);

        try
        {
            //Closes I/O stream if file is null/incompatible
            using (StreamReader reader = fi.OpenText())
            {
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

    void ProcessInputText(List<string> text)
    {
        char[] seperators = new char[]{' ', ',', '(', ')'};

        string[] str_position = text[0].Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);
        string[] str_rotation = text[1].Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

        transform.position = new Vector3(float.Parse(str_position[0]),
            float.Parse(str_position[1]), float.Parse(str_position[2]));

        transform.rotation = new Quaternion(float.Parse(str_rotation[0]), 
            float.Parse(str_rotation[1]), float.Parse(str_rotation[2]), float.Parse(str_rotation[3]));
    }

}
