extends Node
class_name BiomeMap

var height = 16
var width = 32
enum {OCEAN, SEA, BEACH, CLIFF, HILLS, MOUNTAINS, GRASSLANDS, DESERT, SAVANNAH, PLATEAU}
#doing a biome system in this why is pointless
#from now on I need to do types of terrain, e.g high, med and low - spikey, hilly, smooth and all inbetween
#after that I should tie all the biomes together http://parzivail.com/procedural-terrain-generaion/
#then perform procedural hyrdrology on the whole thing.
var biomeCompat = []

func _ready():
	initBiomeTable()
	for x in range(9):
		print(biomeCompat[x])
	
	var array2D = [] # Makes a 2d Array
	for i in range(height):
		array2D.append([])
		for j in range(width):
			array2D.append(0)
	
	#print(array2D) # prints entire array

func initBiomeTable():
	var ocean = [OCEAN, SEA]#can only be adjacent to either sea or ocean
	var sea = [OCEAN, SEA, BEACH, DESERT, CLIFF]# only sea, ocean, beach, desert or cliff
	var beach = [SEA]# has to be adjacent to sea
	var cliff = [SEA, BEACH, HILLS, MOUNTAINS, GRASSLANDS, SAVANNAH, CLIFF]# not adjacent to ocean or desert
	var hills = [BEACH, CLIFF, HILLS, MOUNTAINS, GRASSLANDS, SAVANNAH, PLATEAU]# not adjacent to sea, ocean, desert
	var mountains = [BEACH, CLIFF, HILLS, MOUNTAINS, GRASSLANDS, SAVANNAH, PLATEAU, DESERT]# not adjacent to sea or ocean
	var grasslands = [BEACH, CLIFF, HILLS, MOUNTAINS, GRASSLANDS, PLATEAU]# not adjacen to desert, savhannah, sea or ocean
	var desert = [OCEAN, SEA, BEACH, CLIFF, MOUNTAINS, DESERT, SAVANNAH, PLATEAU]# not adjacent to grasslands or hills
	var savhannah = [BEACH, CLIFF, MOUNTAINS, DESERT, SAVANNAH, PLATEAU]# not adjacent to hills, grasslands, seas or oceans
	var plateau = [CLIFF, HILLS, MOUNTAINS, GRASSLANDS, DESERT, SAVANNAH, PLATEAU]# not adjacent to seas or oceans or beaches
	biomeCompat.append(ocean)
	biomeCompat.append(sea)
	biomeCompat.append(beach)
	biomeCompat.append(cliff)
	biomeCompat.append(hills)
	biomeCompat.append(mountains)
	biomeCompat.append(grasslands)
	biomeCompat.append(desert)
	biomeCompat.append(savhannah)
	biomeCompat.append(plateau)
