using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultGameConfigDomain
{
    public static class DefaultGameConfig 
    {
        //use for contain config data 
        public static float reloadTime = 2f;
        public static float fireInterval = 0.09f;
        public static int Bullet_Capacity = 30;
        public static int BulletRemain_Capacity = 90;

    	public static float MaxSpeed = 5f;
		public static float accelerateScale = 5f;
		//move parameter
		
		//spread parameter in different circumstences
		public static float standSpread = 0.05f;
		public static float movingBaseSpread = 0.1f;
		public static float movingMagnitudeSpread = 0.02f;//vary with the moveTendency
		public static float crouchSpread = 0.03f;
		public static float onLadderSpread = 0.3f;

		//recoil in 2 dimensions
		public static float VerticalRecoilMapping( int aimo ){
			if( aimo <= 7 ){
				return 0.05f;	
			}
			else if( aimo <= 15 ){
				return 0.035f;
			}
			else{
				return 0.045f+Random.Range(-1,1) * 0.05f;
			}
	
		}//linear varying mapping function
		
		public static float HorizontalRecoilMapping( int aimo ){
			float HorizontalRecoil1 = 0.025f;
			float HorizontalRecoil2 = 0.035f;
			int[] MappingArray = { 1 , 1 , -1 , -1 , 1 , 1 , -1 , -1 , -1 , -1 , -1 , 1 , -1 , 1 , 1 , 1 , 1 , 1, 1 , 1 , -1 , 1 , -1 , 1 , -1 , -1 , -1 , -1 , -1 , -1 };
			if( aimo <= 6 ){
				return MappingArray[ aimo ] * HorizontalRecoil1;
			}
			else{
				return MappingArray[ aimo ] *HorizontalRecoil2;
			}
		}

		public static float recoverInterval=0.01f;
		public static float aimResetSpeed=0.03f;
		
    }
}

