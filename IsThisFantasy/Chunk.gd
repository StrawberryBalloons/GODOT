extends Spatial
class_name Chunk
#maybe diamond square noise
var mesh_instance
var noise
var x
var z
var chunk_size
var should_remove = true

func _init(noise, x, z, chunk_size):
	self.noise = noise;
	self.x = x;
	self.z = z;
	self.chunk_size = chunk_size;

func _ready():
	#generate_chunk()
	#generate_water()
	pass

func generate_chunk():
	var plane_mesh = PlaneMesh.new()
	plane_mesh.size = Vector2(chunk_size, chunk_size)
	plane_mesh.subdivide_depth = chunk_size * 0.25
	plane_mesh.subdivide_width = chunk_size * 0.25
	
	#give material
	plane_mesh.material = preload("res://terrain.material")
	
	var surface_tool = SurfaceTool.new()
	var data_tool = MeshDataTool.new()
	surface_tool.create_from(plane_mesh, 0)
	var array_plane = surface_tool.commit()
	var error = data_tool.create_from_surface(array_plane, 0)
	
	for i in range(data_tool.get_vertex_count()):
		var vertex = data_tool.get_vertex(i)
		#vertex.y = noise.get_noise_3d(vertex.x + x, vertex.y, vertex.z + z) * 100 #this is where ti magic happens
		vertex.y = pow(noise.get_noise_3d(vertex.x + x, vertex.y, vertex.z + z) * 10, 2)
		data_tool.set_vertex(i, vertex)
	
	for s in range(array_plane.get_surface_count()):
		array_plane.surface_remove(s)
	
	data_tool.commit_to_surface(array_plane)
	surface_tool.begin(Mesh.PRIMITIVE_TRIANGLES)
	surface_tool.create_from(array_plane, 0)
	surface_tool.generate_normals()

	mesh_instance = MeshInstance.new()
	mesh_instance.mesh = surface_tool.commit()
	mesh_instance.create_trimesh_collision()
	mesh_instance.cast_shadow = GeometryInstance.SHADOW_CASTING_SETTING_OFF
	add_child(mesh_instance)
	
func generate_water():
	var plane_mesh = PlaneMesh.new()
	plane_mesh.size = Vector2(chunk_size, chunk_size)

	#give material
	plane_mesh.material = preload("res://water.material")
	var mesh_instance = MeshInstance.new()
	mesh_instance.mesh = plane_mesh
	add_child(mesh_instance)
