using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

namespace UnityStandardAssets.Vehicles.Car
{
    public class UpdatePlayerVehicle : MonoBehaviour {
        private CustomCarController m_Car; // the car controller we want to use

        private int counter;
        private int bumpCounter;
        private int bumpBackoff;

        public void Awake()
        {
            m_Car = GetComponent<CustomCarController>();
            counter = 0;
            bumpCounter = 0;
            bumpBackoff = 0;
        }

        // Use this for initialization
        void Start()
        {

        }
        
        void Update()
        {

            // Speed Burst
            //
            if (counter > 0)
            {
                counter -= counter > 0 ? 1 : 0;
                m_Car.Move(0f, 100f, 100f, 0f);
            }

            // Car bump
            //
            if (bumpCounter > 0 || bumpBackoff > 0)
            {

                bumpCounter -= bumpCounter > 0 ? 1 : 0;
                bumpBackoff -= bumpBackoff > 0 ? 1 : 0;
                if (bumpCounter > 0)
                    m_Car.Bump();
            }
        }

        void MoveVehicle(string args)
        {
            string[] floats = args.Split(',');

            float steering = float.Parse(floats[0]);
            float acc = float.Parse(floats[1]);
            float brakes = float.Parse(floats[2]);
            int bump = int.Parse(floats[3]);

            if (bumpBackoff == 0 && bump == 1)
            {
                bumpCounter = 10;
                bumpBackoff = 60;
            } else
            {
                // Move(float steering, float accel, float footbrake, float handbrake)
                m_Car.Move(steering, acc, acc, brakes);
            }

            /*
            if (Input.GetKeyDown("f"))
            {
                // Speed burst
                //
                Debug.Log("f pressed");
                counter = 120;
            }
            */
        }
    }
}

