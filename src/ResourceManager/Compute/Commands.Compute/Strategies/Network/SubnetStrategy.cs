﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Management.Internal.Network.Version2017_10_01.Models;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Commands.Common.Strategies.Network
{
    static class SubnetPolicy
    {
        public static NestedResourceStrategy<Subnet, VirtualNetwork> Strategy { get; }
            = NestedResourceStrategy.Create<Subnet, VirtualNetwork>(
                header: "subnets",
                get: (vn, name) => vn.Subnets?.FirstOrDefault(s => s?.Name == name),
                createOrUpdate: (vn, name, subnet) =>
                {
                    subnet.Name = name;
                    if (vn.Subnets == null)
                    {
                        vn.Subnets = new List<Subnet> { subnet };
                        return;
                    }
                    var s = vn
                        .Subnets
                        .Select((sn, i) => new { sn, i })
                        .FirstOrDefault(p => p.sn.Name == name);
                    if (s != null)
                    {
                        vn.Subnets[s.i] = subnet;
                        return;
                    }
                    vn.Subnets.Add(subnet);
                });

        public static NestedResourceConfig<Subnet, VirtualNetwork> CreateSubnet(
            this ResourceConfig<VirtualNetwork> virtualNetwork, string name, string addressPrefix)
            => Strategy.CreateConfig(
                parent: virtualNetwork,
                name: name,
                createModel: () => new Subnet { Name = name, AddressPrefix = addressPrefix });
    }
}
