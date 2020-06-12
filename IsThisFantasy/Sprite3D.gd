extends Sprite3D
class_name HeightMap
var mapX = 512
var mapY = 512
var noiseTex
var noise
# Called when the node enters the scene tree for the first time.
func _ready():
	noise = get_parent().get_parent().get_parent().setNoise()

	#print(noise.get_noise_2d(1.0, 1.0)) #heightmap multiply by 125 and add to 125 to get black to white ratio
	#print(noise.get_noise_3d(0.5, 3.0, 15.0)) #3d simplex
	
	generate_Map()


func generate_Map():
	noiseTex = NoiseTexture.new()
	noiseTex.noise = noise
	
	set_texture(noiseTex)
	
	
	#print("end reached") #3d simplex
