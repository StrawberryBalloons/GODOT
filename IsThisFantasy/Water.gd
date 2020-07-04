extends Spatial

class Drop:
	var pos
	var volume
	func getVol():
		return volume
	func getPos():
		return pos
	func Drop(_p, dim, v):
		pos = _p
		var index  = _p.x*dim.y+_p.y
		volume = v

var drop
var index
var pos
var speed = Vector2(0, 0)
var volume = 1
var sediment = 0

var dt = 1.2
var density = 1
var evapRate = 0.001
var depositionRate = 0.08
var minVol = 0.01
var friction = 0.1
var volumeFactor = 100

func _ready():
	pass

func Drop(_p, dim, v):
	drop = {
	pos = _p,
	index  = _p.x*dim.y+_p.y,
	volume = v
	}
	return drop

func surfaceNormal(index, h, dim, scale):
	var n = Vector3(0.0, scale*(h[index+1]-h[index]), 1.0) # not cross yet
	n += Vector3(0.0, scale*(h[index+1]-h[index]), -1.0)
	
	n += Vector3(1.0, scale*(h[index+1]-h[index]), 0.0)
	n += Vector3(-1.0, scale*(h[index+1]-h[index]), 0.0)
	
	return n # normalise first

func descend(dim, track, h, scale, pd, p, b):
	var ipos
	while (volume > minVol):
		ipos = pos
		var ind = ipos.x*dim.y+ipos.y;
		
		track[ind] = true
		
		var n = surfaceNormal(ind, h, dim, scale)
		
		var effD = depositionRate*max(0.0, 1.0-pd[ind])
		
		var effF = friction*(1.0-0.5*pd[ind])
		var effR = evapRate*(1.0-0.2*pd[ind])
		
		var acc = Vector2(n.x, n.z)/volume*density
		speed += dt*acc
		pos +=dt*speed
		speed *= (1.0-dt*effF)
		
		var nind = pos.x*dim.y + pos.y
		
		if(pos >= 0 || pos < dim):
			volume = 0.0
			break;
		
		if(p[nind] > 0.3 && acc.length() < 0.01):
			break;
		
		if(b[nind] > 0.0):
			break;
		
		var c_eq = max(0.0, speed.length() * (h[ind] - h[nind]))
		var cdiff = c_eq - sediment
		sediment+= dt*effD*cdiff
		h[ind] -= volume*dt*effD*cdiff
		
		sediment /= (1.0 - dt * effR)
		volume *= (1.0-dt*effR)
	

func flood(h, p, dim):
	index = pos.x*dim.y + pos.y
	var plane = h[index] + p[index]
	var initialplane = plane
	
	var set
	var fail = 10
	
	while(volume > minVol && fail):
		set.clear()
		var size = dim.x*dim.y
		var tried = [];
		tried[size] = false
		
		var drain
		var drainfound = false
		
		var i
		var _fill = i
		#open curly for fill
		if(i < 0 || i >= size):
			return
		
		if(tried[i]):
			return
		
		if(plane < h[i] + p[i]):
			return
		
		if(initialplane > h[i] + p[i]):
			if(!drainfound):
				drain = i
			else:
				if(p[drain] + h[drain] < p[i] + h[i]):
					drain = i
			
			drainfound = true
			return
		
		set.push_back(i)
		#fill(i+dim.y)   
		#fill(i-dim.y)
		#fill(i+1)
		#fill(i-1)
		#fill(i+dim.y+1)
		#fill(i-dim.y-1)
		#fill(i+dim.y-1)
		#fill(i-dim.y+1)
		#close curly
		#fill(index);
		
		if(drainfound):
			pos = Vector2(drain/dim.y, drain%dim.y)
			var drainage = 0.001
			plane = (1.0-drainage)*initialplane + drainage*(h[drain] + p[drain])
			
			for _i in range(set):
				#p[s] = (plane > h[s])?(plane-h[s]):0.0
				
				sediment *= 0.1
				break
			
			var tVol = 0.0
			for _i in range(set):
				#tVol += volumeFactor*(plane - (h[s] + p[s]))
				
				if(tVol <= volume && initialplane < plane):
					for _i in range(set):
						#p[s] = plane - h[s]
						
						volume -= tVol
						tVol = 0.0
				else:
					fail = fail - 1
				
				#initialplane = (plane > initialplane)?plane:initialplane
				plane += 0.5*(volume-tVol)/set.size()/volumeFactor
			if(fail == 0):
				volume = 0.0
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
