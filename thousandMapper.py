#!/usr/bin/env python
import sys
 
#--- get all lines from stdin ---
for line in sys.stdin:
    #--- remove leading and trailing whitespace---
    line = line.strip()

    #--- split the line into words ---
    # words = line.split()

    
    newyorktimes = line
    nyt_results = newyorktimes['results']
	res = nyt_results[0]
	thousand = res['thousand_best']


	print '%s\t%s' % (thousand, "1")