CheckService
============

This little program performs an HTTP GET on a service on a specific host and returns the HTTP status code together with the body text returned by the service. Optionally it can tunnel through a load balancer to a particular underlying host.

Changelog
---------

1.1.0   Open-source and upload to Github

1.0.2   Support sites with duff SSL certificates (and moan about them, too)

1.0.1   Allow specification of alternate ports, e.g. <http://foo.bah:8080/>

1.0.0   Initial version

Enjoy,

Jeremy McGee

mailto:<jeremy@familymcgee.com>

---

TODO
----

* Needs migration to a later version of .NET
* Needs to be built under Visual Studio Code
