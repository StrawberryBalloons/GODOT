extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.
# Verb rune = V
# Element Rune = E
# World rune = W
# Assist rune = A
#RUNE STRING EXAMPLES
# W -> V -> E (local gather mana)
# (assume W if not specificed) -> V -> E (world transmute fire) #converts magic to fire if no prior element specified
# alt form from above V -> E -> V -> E, or use multiple instances instead of one large string?
# so string spell could be VEVE and it would read VE and VE as separate, or just have it linger for another to interact?
#
# W -> V -> E -> A (assist runes have special effects, maybe can go A->A->A->A multiply intercept closest projectiles
# or
# A->V->A->A multiply target all enemies), if i want to string A's together I need to rethink how they work
# just saying each A affects the spell is foolish and won't string into a V, however A-V-A is doable I think, but A-V-A-A is not
# 
# AOE->V->A (intercept projectiles in an area, however, needs to be attached to a spell)
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
# transmute # mana to element or element to element
# Form #attaches element to the form the runes apply to e.g over body parts or over a weapon
# On hit
# target
# Detect
# intercept
# Sustain #to make continuous
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
# Closest
# Furthest
# all
#
# Complex ASSISTANCE RUNES
# See #able to see types of elements, e.g see life makes you able to see hp levels in creatures
# projectile # incoming or outgoing projectiles?
# Bond #takes the xyz coordinates of someone or something marked
# Connect #connects two spells (mainly for gravity magic)
# multiply
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
