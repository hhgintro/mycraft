using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MyCraft;


namespace FactoryFramework
{
	//Drill
	public class Driller : Building, IOutput
	{
		//private Dictionary<int/*itemid*/, int/*amount*/> _outputs = new Dictionary<int, int>();
		private Dictionary<int/*itemid*/, MyCraft.BuildingItem> _outputs = new Dictionary<int, MyCraft.BuildingItem>();
		//public List<MyCraft.BuildingPanel> _panels = new List<MyCraft.BuildingPanel>();


		[SerializeField] private float _resourcesPerSecond = 1f;
		private float secondsPerResource { get { return 1f / _resourcesPerSecond; } }
		private float _t;

		//public void SetOutputResource(Item item)
		//{
		//    if (item == resource.itemStack.item) 
		//        return;
		//    resource.itemStack.amount = 0;
		//    resource.itemStack.item = item;
		//    _t = 0f;
		//}
		public override void InitStart()
		{
			// use mesh to calculate bounds
			Mesh m = this.transform.GetChild(0).GetComponent<MeshFilter>()?.mesh;
			Vector3 center = (m != null) ? m.bounds.center : transform.position;
			Vector3 size = (m != null) ? m.bounds.extents : Vector3.one;
			Vector3 point = transform.TransformPoint(center);
			foreach (Collider c in Physics.OverlapBox(point, size * 1.125f))
			{
				if (c.TryGetComponent(out Resource r))
				{
					//this.SetOutputResource(r.item);

					//채광할 수 있는 광물정보를 등록합니다.
					if (0 < _outputs.Count) _outputs.Clear();
					_outputs.Add(r.itemid, new MyCraft.BuildingItem());
					break;
				}
			}

			base.InitStart();
		}
		public override void SetEnable_2(bool enable)
		{
			base.SetEnable_2(enable);
			if (false == enable) return;

			// use mesh to calculate bounds
			Mesh m = this.transform.GetChild(0).GetComponent<MeshFilter>()?.mesh;
			Vector3 center = (m != null) ? m.bounds.center : transform.position;
			Vector3 size = (m != null) ? m.bounds.extents : Vector3.one;
			Vector3 point = transform.TransformPoint(center);
			foreach (Collider c in Physics.OverlapBox(point, size*1.125f))
			{
				if (c.TryGetComponent(out Resource r))
				{
					//this.SetOutputResource(r.item);

					//채광할 수 있는 광물정보를 등록합니다.
					if (0 < _outputs.Count) _outputs.Clear();
					_outputs.Add(r.itemid, new MyCraft.BuildingItem());
					break;
				}
			}

			//this._panels.Add(new MyCraft.BuildingPanel((byte)this._panels.Count, 0));// base._itembase._assembling.inputs));//input
			//this._panels.Add(new MyCraft.BuildingPanel((byte)this._panels.Count, 1));//output
			////this._panels.Add(new BlockSlotPanel(this._panels.Count, ((MachineItemBase)base._itembase)._assembling.chips));//chip

		}

		public override void ProcessLoop()
		{
			//if (resource.itemStack.item == null)
			//{
			//    IsWorking = false;
			//    return;
			//}
			//// maybe move this to coroutine or async
			//if (resource.itemStack.amount == resource.itemStack.item.itemData.maxStack)
			//{
			//    IsWorking = false;
			//    return;
			//}

			//IsWorking = true;
			//_t += Time.deltaTime * PowerEfficiency; // FIXME maybe
			//if (_t > secondsPerResource)
			//{
			//    resource.itemStack.amount += 1;
			//    _t = _t % secondsPerResource;
			//}

			if (0 == _outputs.Count)
			{
				_IsWorking = false;
				return;
			}

			//is full
			var output = _outputs.ElementAt(0);
			MyCraft.ItemBase itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(output.Key);
			if (itembase.Stackable <= _outputs[output.Key]._amount)
			{
				_IsWorking = false;
				return;
			}

			_IsWorking = true;
			_t += Time.deltaTime * PowerEfficiency; // FIXME maybe
			if (_t > secondsPerResource)
			{
				_outputs[output.Key]._amount += 1;
				_t = _t % secondsPerResource;
			}
		}

		#region GIVE_OUTPUT
		//HG[2023.06.09] Item -> MyCraft.ItemBase
		//public bool CanGiveOutput(Item filter = null)
		//{
		//    if (filter != null) Debug.LogWarning("Producer Does not Implement Item Filter Output");
		//    return resource.itemStack.item != null && resource.itemStack.amount > 0;
		//}
		public bool CanGiveOutput(OutputSocket cs = null)
		{
			if(0 == _outputs.Count) return false;
			var output = _outputs.ElementAt(0);
			if (_outputs[output.Key]._amount <= 0) return false;
			return true;
		}

		//public Item OutputType() { return resource.itemStack.item; }
		public int OutputType(OutputSocket cs = null) {
			if (0 == _outputs.Count) return 0;
			var output = _outputs.ElementAt(0);
			return output.Key;
		}

		//public Item GiveOutput(Item filter = null)
		//{
		//    if (filter != null) Debug.LogWarning("Producer Does not Implement Item Filter Output");
		//    if (resource.itemStack.item == null || resource.itemStack.amount == 0) return null;
		//    resource.itemStack.amount -= 1;
		//    return resource.itemStack.item;
		//}
		public int GiveOutput(OutputSocket cs = null)
		{
			if (0 == _outputs.Count) return 0;
			var output = _outputs.ElementAt(0);
			if (_outputs[output.Key]._amount <= 0) return 0;
			_outputs[output.Key]._amount -= 1;
			return output.Key;
		}
		//..//HG[2023.06.09] Item -> MyCraft.ItemBase
		#endregion //..GIVE_OUTPUT

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireSphere(Vector3.zero, 1f);
		}
	}
}