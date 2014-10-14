using UnityEngine;
using System.Collections;

public class PhotonBazooka : Photon.MonoBehaviour {

	protected Animator animator;
    public GameObject targetA = null;
    public GameObject leftHandle = null;
    public GameObject rightHandle = null;
    public GameObject bazoo = null;
    public GameObject bullet = null;
    public GameObject spawm = null;

    private bool load = false;

    void OnGUI()
    {
        GUILayout.Label("Try to kill the bear!");
		GUILayout.Label("Use Fire1 to shoot bullets");
		GUILayout.Label("Use Fire2 to toggle aiming with bazooka");
    }
	
	private Vector3 correctPlayerPos;
	private float h;
	private float v;
	
	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
		
		targetA = GameObject.Find("Teddy");
		bullet = GameObject.Find("Bullet");
	}
	
	// Update is called once per frame
	void Update () 
	{
		float aim = 0.0f;
		float fire = 0.0f;
		
		if (animator)
		{
			animator.SetFloat("Aim", load ? 1 : 0, .1f, Time.deltaTime);
			
			aim = animator.GetFloat("Aim");
			fire = animator.GetFloat("Fire");
			
			animator.SetFloat("Fire",0, 0.1f, Time.deltaTime);
			
			animator.SetFloat("Speed", h*h+v*v);
			animator.SetFloat("Direction", h, 0.25f, Time.deltaTime);
		}
		
		if (photonView.isMine)
		{
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
			
			if (Input.GetButton("Fire2"))
			{
				if (load && aim > 0.99) { load = false; }
				else if (!load && aim < 0.01) load = true;
			}
			
			if (Input.GetButton("Fire1") && fire < 0.01 && aim > 0.99)
			{
				photonView.RPC("Fire", PhotonTargets.All);
			}
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(h);
			stream.SendNext(v);
			stream.SendNext(load);
		}
		else
		{
			//Network player, receive data
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			h = (float)stream.ReceiveNext();
			v = (float)stream.ReceiveNext();
			load = (bool)stream.ReceiveNext();
		}
	}

	[RPC]
	void Fire()
	{
		animator.SetFloat("Fire",1);
		
		if (bullet != null && spawm != null)
		{
			GameObject newBullet = Instantiate(bullet, spawm.transform.position , Quaternion.Euler(0, 0, 0)) as GameObject;
			
			Rigidbody rb = newBullet.GetComponent<Rigidbody>();
			
			if (rb != null)
			{
				rb.velocity = spawm.transform.TransformDirection(Vector3.forward * 20);
			}
		}
	}

    void OnAnimatorIK(int layerIndex)
    {
        float aim = animator.GetFloat("Aim");

        // solve lookat and update bazooka transform on first il layer
        if (layerIndex == 0)
        {
            if (targetA != null)
            {
                Vector3 target = targetA.transform.position;

                target.y = target.y + 0.2f * (target - animator.rootPosition).magnitude;

                animator.SetLookAtPosition(target);
                animator.SetLookAtWeight(aim, 0.5f, 0.5f, 0.0f, 0.5f);

                if (bazoo != null)
                {
                    float fire = animator.GetFloat("Fire");
                    Vector3 pos = new Vector3(0.195f, -0.0557f, -0.155f);
                    Vector3 scale = new Vector3(0.2f, 0.8f, 0.2f);
                    pos.x -= fire * 0.2f;
                    scale = scale * aim;
                    bazoo.transform.localScale = scale;
                    bazoo.transform.localPosition = pos;
                }        
 
            }
        }

        // solve hands holding bazooka on second ik layer
        if (layerIndex == 1)
        {
            if (leftHandle != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandle.transform.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandle.transform.rotation);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, aim);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, aim);
            }

            if (rightHandle != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandle.transform.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandle.transform.rotation);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, aim);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, aim);
            }
        }
    }
}
