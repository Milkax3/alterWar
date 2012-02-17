-- phpMyAdmin SQL Dump
-- version 3.3.9
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Erstellungszeit: 17. Februar 2012 um 18:04
-- Server Version: 5.5.8
-- PHP-Version: 5.3.5

--
-- (c) Copyright 2012 alterWar exported database ... Supreme skillzZz... Greez from hell :D
--
SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Datenbank: `wremu`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `account`
--

CREATE TABLE IF NOT EXISTS `account` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `passwd` varchar(16) NOT NULL,
  `nickname` varchar(50) NOT NULL,
  `email` varchar(50) NOT NULL,
  `accesslevel` int(1) NOT NULL DEFAULT '1',
  `online` int(1) NOT NULL DEFAULT '0',
  `awc` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

--
-- Daten für Tabelle `account`
--


-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `account_detail`
--

CREATE TABLE IF NOT EXISTS `account_detail` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `exp` int(100) NOT NULL,
  `level` int(3) NOT NULL,
  `dinar` int(32) NOT NULL,
  `kills` int(32) NOT NULL,
  `death` int(32) NOT NULL,
  `Premium` int(1) NOT NULL,
  `PremiumTime` bigint(11) NOT NULL,
  `PremiumDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `PremiumLeft` int(11) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

--
-- Daten für Tabelle `account_detail`
--


-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `inventory`
--

CREATE TABLE IF NOT EXISTS `inventory` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userId` bigint(11) NOT NULL,
  `itemCode` varchar(8) NOT NULL,
  `expireDate` bigint(64) NOT NULL,
  `isPX` enum('True','False') NOT NULL DEFAULT 'False',
  `engineer` varchar(23) NOT NULL DEFAULT '10,20,30,40,50,60,70,80',
  `medic` varchar(23) NOT NULL DEFAULT '10,20,30,40,50,60,70,80',
  `sniper` varchar(23) NOT NULL DEFAULT '10,20,30,40,50,60,70,80',
  `assault` varchar(23) NOT NULL DEFAULT '10,20,30,40,50,60,70,80',
  `heavy` varchar(23) NOT NULL DEFAULT '10,20,30,40,50,60,70,80',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

--
-- Daten für Tabelle `inventory`
--

