﻿//
// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       thomass
//
// Copyright 2004-2011 by OM International
//
// This file is part of OpenPetra.org.
//
// OpenPetra.org is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// OpenPetra.org is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with OpenPetra.org.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Ict.Petra.Shared.MCommon.Data;
using Ict.Petra.Shared.MPartner;
using Ict.Petra.Shared.MPartner.Partner.Data;


namespace SampleDataConstructor
{
	/// <summary>
	/// This class bundles executive summary of the last action
	/// (e.g. in human readable form).
	/// </summary>
	class ExecutionReport
	{
		string lastActionHumanReadable;
		public ExecutionReport(string lastActionHumanReadable)
		{
			this.lastActionHumanReadable = lastActionHumanReadable;
		}
		public override string ToString()
		{
			return lastActionHumanReadable;
		}
	}
	
	/// <summary>
	/// SampleDataConstructors main class: Creates a TDS from Raw Data
	/// </summary>
	/// TODO: unhappy that I duplicated all the code from ImportExportYml.cs. What should one do instead?
	class DataBuilder : RawData
	{
		private static Int64 newPartnerKey = -1; 
		/// <summary>
		/// This gets a new Partner Key.
		/// </summary>
		/// <remarks>
		/// Done this way in ImportExportYml.cs
		/// TODO: where should the partnerKey _actually_ come from?
		/// </remarks>
		protected static Int64 getNewPartnerKey()
		{
			Int64 partnerKey = newPartnerKey;
	        newPartnerKey--;
	        return partnerKey;
		}
		
		private static Int32 newLocationKey = -1; 
		/// <summary>
		/// This gets a new Location Key.
		/// </summary>
		/// <remarks>
		/// TODO: where should the locationKey _actually_ come from?
		/// </remarks>
		protected static Int32 getNewLocationKey()
		{
			Int32 locationKey = newLocationKey;
	        newLocationKey--;
	        return locationKey;
		}
		
	
		public static PPartnerRow createNewPartner(SampleDataConstructorTDS dataTDS)
		{
			PPartnerRow partner = dataTDS.PPartner.NewRowTyped();
			partner.PartnerKey = getNewPartnerKey();
			partner.StatusCode = "ACTIVE";
			return partner;
		}
	
		public static PFamilyRow createNewFamily(SampleDataConstructorTDS dataTDS, RPerson person)
		{
			PFamilyRow family = dataTDS.PFamily.NewRowTyped();
			family.FirstName  = person.FirstName;
			family.FamilyName = person.FamilyName;
			family.Title      = person.Title;
			family.CreatedBy  = "DemoData";
			family.DateCreated = DateTime.Now;
			//// family.FieldKey
			//// family.MaritalStatus
			//// family.MaritalStatusComment
			//// family.MaritalStatusSince
			return family;
		}
		
		// TOCHECK: why sometimes organization (with a "z") and sometimes organisation (with "s")?
		public static POrganisationRow createNewOrganisation(SampleDataConstructorTDS dataTDS, ROrganization organization)
		{
       		POrganisationRow organisationRow = dataTDS.POrganisation.NewRowTyped();
            organisationRow.OrganisationName = organization.Name;
            return organisationRow;
		}
		
		public static PLocationRow createNewLocation(
			SampleDataConstructorTDS dataTDS, RLocation location)
		{
            PLocationRow locationRow = dataTDS.PLocation.NewRowTyped(true);
            locationRow.LocationKey = getNewLocationKey();
            locationRow.SiteKey = 0;

            locationRow.CountryCode = location.CountryCode;
            locationRow.StreetName = location.Addr2;
            locationRow.City = location.City;
            locationRow.PostalCode = location.PostCode;
            return locationRow;
		}
            
		public static PPartnerLocationRow createNewPartnerLocation(
			SampleDataConstructorTDS dataTDS,
			RLocation location
		)
		{	
            PPartnerLocationRow partnerLocationRow = dataTDS.PPartnerLocation.NewRowTyped(true);
            partnerLocationRow.SiteKey = 0;
            partnerLocationRow.SendMail = true;
            partnerLocationRow.DateEffective = DateTime.Now;
            partnerLocationRow.LocationType = "HOME";
            
            // TODO: fill in from person
            // can't we just link email to family? Or have an "email" type?
            // partnerlocationRow.EmailAddress = 
            
            partnerLocationRow.TelephoneNumber = location.Phone;
            
            // TODO: fill in from mobile phone
            // partnerlocationRow.MobileNumber = 
            
            return partnerLocationRow;
		}
	
			
		public static void couple(PPartnerRow partner, PFamilyRow family)
		{
			// partner: data associated with family
	        partner.PartnerClass = MPartnerConstants.PARTNERCLASS_FAMILY;
	        partner.AddresseeTypeCode = MPartnerConstants.PARTNERCLASS_FAMILY;
	
	        partner.PartnerShortName =
	        	Calculations.DeterminePartnerShortName(family.FamilyName,family.Title,family.FirstName);
	 		family.PartnerKey = partner.PartnerKey;
		}
		
		public static void couple(PPartnerRow partnerRow, POrganisationRow organisationRow)
		{
			organisationRow.PartnerKey = partnerRow.PartnerKey;
     		partnerRow.PartnerShortName = organisationRow.OrganisationName;
            partnerRow.PartnerClass = MPartnerConstants.PARTNERCLASS_ORGANISATION;
		}
		
		public static void couple(
			PLocationRow locationRow, 
			PPartnerLocationRow partnerLocationRow
		)
		{
			partnerLocationRow.LocationKey = locationRow.LocationKey;   
		}
		
		public static void couple(
			PPartnerRow partnerRow,
			PPartnerLocationRow partnerLocationRow
		)
		{
			partnerLocationRow.PartnerKey = partnerRow.PartnerKey;
		}
	
		
		public static List<PPartnerTypeRow> createSpecialTypes(SampleDataConstructorTDS dataTDS, RPerson person)
		{
			// new StringCollection("VOLUNTEER","SUPPORTER");
			// No Special Types are created for now.
			StringCollection specialTypes = new StringCollection();
			var partnerTypes = new List<PPartnerTypeRow>();
			foreach (string specialType in specialTypes) {
				PPartnerTypeRow partnerType = dataTDS.PPartnerType.NewRowTyped();
		        partnerType.TypeCode = specialType.Trim();
		        partnerTypes.Add(partnerType);
			}
			return partnerTypes;
		}
	
		public static void couple(PPartnerRow partner, List<PPartnerTypeRow> specialTypes)
		{
	        foreach (PPartnerTypeRow partnerType in specialTypes)
	        {
	            partnerType.PartnerKey = partner.PartnerKey;                 
	            // TODO: check if special type does not exist yet, and create it
	        }				
		}
	
		/// <summary>
		/// Add People from raw data to given TDS.
		/// </summary>
		/// <remarks>
		/// Based on ImportExportYml.cs 
		/// </remarks>
		public static void initPeople(
			SampleDataConstructorTDS rawDataTDS, 
			RawData rawData,
			out ExecutionReport report
		)
		{
			int numEntries = rawData.People.Count;
			foreach (RPerson rPerson in rawData.People) {
				var partner = createNewPartner(rawDataTDS);
				var family = createNewFamily(rawDataTDS,rPerson);
				couple(partner,family);
			 	
				rawDataTDS.PPartner.Rows.Add(partner);
				rawDataTDS.PFamily.Rows.Add(family);
	
				var specialTypes = createSpecialTypes(rawDataTDS,rPerson);			
				couple(partner,specialTypes);
				
			
				/*
				foreach (PPartnerTypeRow specialType in specialTypes) {
					rawDataTDS.PPartnerType.Rows.Add(specialType);
				}
			*/
				
				//// unused: 
				// person.DateOfBirth
				// person.Email
			}
			report = new ExecutionReport(
				"Added "+numEntries+" people (partner+family) from raw data"
			);
		}

	/// <summary>
	/// Add Organisations from raw data to given TDS.
	/// </summary>
	public static void initOrganisations(
		SampleDataConstructorTDS rawDataTDS, 
		RawData rawData,
		out ExecutionReport report
	)
	{
		int numEntries = rawData.Organizations.Count;
		foreach (ROrganization rOrganisation in rawData.Organizations) {
			var partner = createNewPartner(rawDataTDS);
			var organisationRow = createNewOrganisation(rawDataTDS,rOrganisation);
			couple(partner,organisationRow);
		 	
			rawDataTDS.PPartner.Rows.Add(partner);
			rawDataTDS.POrganisation.Rows.Add(organisationRow);
		}
		report = new ExecutionReport(
			"Added "+numEntries+" organisation (partner+organisation) from raw data"
		);
	}		
			
	/// <summary>
		/// Add Locations from raw data to given TDS.
		/// </summary>
		/// <remarks>
		/// Based on ImportExportYml.cs 
		/// </remarks>
		public static void initLocations(
			SampleDataConstructorTDS rawDataTDS,
			RawData rawData,
			out ExecutionReport report
		)
		{
			int numEntries = rawData.Locations.Count;
			
			foreach (RLocation location in rawData.Locations) {
				var locationRow = createNewLocation(rawDataTDS,location);
				var partnerLocationRow = createNewPartnerLocation(rawDataTDS,location);
				
				couple(locationRow,partnerLocationRow);
			 	
				rawDataTDS.PLocation.Rows.Add(locationRow);
      			rawDataTDS.PPartnerLocation.Rows.Add(partnerLocationRow);

			}
			report = new ExecutionReport(
				"Added "+numEntries+" Locations (locations+partnerlocations) from raw data"
			);
		}
	
		// public static assignConditional
		
		public static void GivePeopleHomes(
			SampleDataConstructorTDS MainTDS,
			SampleDataConstructorTDS UnusedLocationsTDS,
			ConstructionStats stats,
			out ExecutionReport report
		)
		{
			// We create an array of unused partnerlocations.
			// From this we will extract the elements.
			Stack<PPartnerLocationRow> partnerLocationsUnused =
				new Stack<PPartnerLocationRow>();
			foreach (PPartnerLocationRow pl in UnusedLocationsTDS.PPartnerLocation.Rows)
				partnerLocationsUnused.Push(pl);
			
			// Attach people to homes
			long cntAll = 0;
			long cntAssigned = 0;
			Random rand = new Random(1);
			foreach (PPartnerRow partnerRow in MainTDS.PPartner.Rows) {
				if (rand.NextDouble() < stats.PeopleWithHomeKnown)
				{
					throw new Exception("Stopped implementing here");
					PPartnerLocationRow pl = partnerLocationsUnused.Pop();
					
					cntAssigned++;
				}
				
				cntAll++;
			}
			
			report = new ExecutionReport("Did stuff");
		}
		
	}
}
