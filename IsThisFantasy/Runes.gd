extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.
#
#RUNE STRING EXAMPLES
# Local Gather Fire Sphere XYZ ROT aoe
# composite rune string? local mana circle - repel mana- intercept projectiles 
# repel mana may not be necessary due to mana circle having a physical presence, otherwise it will repel mana only?
#
#rune keywords:
# 
# VERB RUNES # only affect elements
# Gather (pulls mana from area)
# Draw (gather from a container)
# Throw  #gives  velocity
# Repel #reflect type of element (shield)
# Break #use this if you want your runes to break things
# Shape #shapes the already existing element
# Disperse #disperses mana 
# Convert # mana to element
# Form #attaches element to the form the runes apply to e.g over body parts or over a weapon
# On hit
# Detect
# Enhance #increase value, based on mana e.g enhance agility sustain 10 #sustains enhance agility at 10 mana per second
# 
# WORLD RUNES #Which spatial coordinates to use
# world space
# local space
# Body or item part, e.g gather mana convert fire blade
# Rotation
# x, y, z coordinates
# area of effect
# Looking Direction 
#
# ELEMENT RUNES #the types of elements that can be used
# Gravity #stretch xyz cells and enact a force upon an area
# Space #manipulate spatial xyz coords
# Mana #particles floating in the air everywhere
# Sound # no idea
# 4 elements
# Light #controls light sources
# Dark #controls shadows
# Life #linked to entities hp
# Death #not sure
# 
# Simple ASSISTANCE RUNES #misc runes to shape or form the magic in some way
# Shapes # sphere, cube, wall etc
# Stats # the stat sheets of those within range
# enemies #affects people designated as not friends
# Allies #affects people designated as friends
# External #uses external mana
# Internal #uses internal mana reservoir
# Sustain #to make continuous
# Closest
# Furthest
#
# Complex ASSISTANCE RUNES
# See #able to see types of elements, e.g see life makes you able to see hp levels in creatures
# projectile # incoming or outgoing projectiles?
# intercept
# Bond #takes the xyz coordinates of someone or something marked
# Connect #connects two spells (mainly for gravity magic)
#
#
#
#
#
#
#
#
#
#
#
#
#
