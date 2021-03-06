﻿/* InstantVR Kinect 2 foot controller
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.4.8
 * date: May 29, 2016
 * 
 * - enabled extrapolation
 */

using UnityEngine;

namespace IVR {

    public class IVR_Kinect2Foot : IVR_Controller {
#if (UNITY_STANDALONE_WIN)
        private IVR_Kinect2 ivrKinect;
        private IVR_Kinect2.BoneType ankleBone;//, footBone;

        void Start() { }

        public void StartTarget(IVR_Kinect2 ivrKinect, bool present, InstantVR ivr) {
            this.ivrKinect = ivrKinect;

            extrapolation = true;

            if (this.transform == ivr.leftFootTarget) {
                ankleBone = IVR_Kinect2.BoneType.AnkleLeft;
            } else {
                ankleBone = IVR_Kinect2.BoneType.AnkleRight;
            }
        }

        public override void UpdateController() {
            if (!ivrKinect.present || !enabled)
                return;

            if (!ivrKinect.GetNewFrame())
                return;

            Vector3 anklePos = Vector3.zero;

            if (ivrKinect.BoneTracking(ankleBone)) {
                anklePos = ivrKinect.GetBonePos(ankleBone);

                controllerRotation = Quaternion.identity;
                controllerPosition = anklePos;

                if (!tracking && ivrKinect.Tracking)
                    tracking = true;

                base.UpdateController();

            } else {
                tracking = false;
            }
        }
#endif
    }
}