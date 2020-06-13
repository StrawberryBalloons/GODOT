extends Spatial


var h = 256
var w = 256
var dim = Vector2(h, w)
var SEED = 0
var sCale = 40
var heightMap = []
var cycles
#var waterPath[h*w] = {0,0}
#var waterPool[h*w] = {0,0}
#var plantDensity[h*2] = {0,0}
var active = false

func _ready():
	#generate()
	#erode(cycles)
	#grow()
	


func generate():
	var noise = get_parent().get_parent().get_parent().setNoise()
	var minimum = 0
	var maximum = 0
	for i in range(h * w):
		heightMap.append(noise.get_noise_3d((i/h) * (1/w), (i%h) * (1/h), SEED))
		if(heightMap[i] > maximum):
			maximum = heightMap[i]
		if(heightMap[i] < minimum):
			minimum = heightMap[i]
	
	for i in range (h*w):
		heightMap[i] = (heightMap[i] - minimum) / (maximum - minimum)
	

func erode(cycles):
	var track = []
	track.append(false)
	for i in range (cycles):
		var newpos = Vector2(randi()%w, randi()%h)
		#Drop drop(newpos)
		var spill = 5
	
	pass

