-- phpMyAdmin SQL Dump
-- version 4.4.9
-- http://www.phpmyadmin.net
--
-- Host: localhost:3306
-- Generation Time: Sep 05, 2015 at 12:49 PM
-- Server version: 5.1.73-cll-lve
-- PHP Version: 5.5.21

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `dbTech`
--

-- --------------------------------------------------------

--
-- Table structure for table `tblCalls`
--

CREATE TABLE IF NOT EXISTS `tblCalls` (
  `code` int(11) NOT NULL,
  `charsType` varchar(5) NOT NULL,
  `callType` tinyint(4) NOT NULL,
  `callDateTime` int(11) NOT NULL,
  `isContinue` tinyint(1) NOT NULL,
  `custName` varchar(60) NOT NULL,
  `siteName` varchar(60) NOT NULL,
  `projName` varchar(60) NOT NULL,
  `address` varchar(60) NOT NULL,
  `id` varchar(60) NOT NULL,
  `location` varchar(60) NOT NULL,
  `recieverServiceName` varchar(25) NOT NULL,
  `callDesc` varchar(480) NOT NULL,
  `callStatus` tinyint(4) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblCalls`
--

INSERT INTO `tblCalls` (`code`, `charsType`, `callType`, `callDateTime`, `isContinue`, `custName`, `siteName`, `projName`, `address`, `id`, `location`, `recieverServiceName`, `callDesc`, `callStatus`) VALUES
(444, 'Ansii', 1, 1434992100, 1, 'קסטרו', 'קניון רמת גן', '', 'דרך שמחה גולן 54 חיפה', 'זיהוי', 'צפון', 'יוסי נסיון', 'לפי דוח ביצוע 301424, החלפת 4 גמישים ב- 0 היחידות', 2),
(15493586, 'Ansii', 1, 1436861700, 0, 'גולף א.קו בע"מ', 'שפיים ( גולף)', '', 'שפיים שפיים', 'זיהוי', '', '', 'רייח באזור שירותים עפי רוני הוך', 6),
(15494175, 'Oem', 1, 1438587300, 0, 'מרכז רפואי העמק', 'בית חולים העמק', '', 'עפולה', 'זיהוי', '', '', 'צילר חדר אוכל תקלה', 33);

-- --------------------------------------------------------

--
-- Table structure for table `tblParts`
--

CREATE TABLE IF NOT EXISTS `tblParts` (
  `code` varchar(16) NOT NULL,
  `partDesc` varchar(60) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `tblRep`
--

CREATE TABLE IF NOT EXISTS `tblRep` (
  `callCode` int(11) NOT NULL,
  `isEnded` tinyint(1) NOT NULL,
  `dtCallConfirm` int(11) NOT NULL,
  `dtStartRide` int(11) NOT NULL,
  `dtStartWork` int(11) NOT NULL,
  `dtEndWork` int(11) NOT NULL,
  `summary` text NOT NULL,
  `recommendation` text NOT NULL,
  `servedName` text NOT NULL,
  `signerName` text NOT NULL,
  `signerRole` text NOT NULL,
  `signPicFileName` text NOT NULL,
  `audioFilesName` text NOT NULL,
  `videoFilesName` text NOT NULL,
  `id` int(11) NOT NULL
) ENGINE=MyISAM AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblRep`
--

INSERT INTO `tblRep` (`callCode`, `isEnded`, `dtCallConfirm`, `dtStartRide`, `dtStartWork`, `dtEndWork`, `summary`, `recommendation`, `servedName`, `signerName`, `signerRole`, `signPicFileName`, `audioFilesName`, `videoFilesName`, `id`) VALUES
(15493586, 1, 1441371664, 1441366755, 1441370186, 1441439837, 'דגדגדגד\nדגדשדשדשדשד\nכככ', 'דגדגדגדג', '', '', '', '', '', '', 6),
(15494175, 0, 1441373054, 1441395306, 1441441031, 0, 'גדגדגדגדג', 'אבדגה \nוטחע', '', '', '', '', '', '', 9),
(444, 0, 0, 0, 0, 0, '', '', '', '', '', '', '', '', 11);

-- --------------------------------------------------------

--
-- Table structure for table `tblSuppParts`
--

CREATE TABLE IF NOT EXISTS `tblSuppParts` (
  `callCode` int(11) NOT NULL,
  `rowCode` int(11) NOT NULL,
  `itemCode` varchar(16) NOT NULL,
  `unit` varchar(8) NOT NULL,
  `quantity` int(11) NOT NULL,
  `remarks` text NOT NULL,
  `id` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `tblTest`
--

CREATE TABLE IF NOT EXISTS `tblTest` (
  `val` varchar(60) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblTest`
--

INSERT INTO `tblTest` (`val`) VALUES
('נסיון'),
('1'),
('12333333'),
('12345678'),
('12345679'),
('15493578'),
('15493586'),
('35431511'),
('12333333'),
('12345678'),
('12345679'),
('15493578'),
('15493586'),
('12333333'),
('12345678'),
('12345679'),
('15493578'),
('15493586');

-- --------------------------------------------------------

--
-- Table structure for table `tblUsers`
--

CREATE TABLE IF NOT EXISTS `tblUsers` (
  `userCode` varchar(15) NOT NULL,
  `userName` varchar(60) NOT NULL,
  `pass` varchar(15) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblUsers`
--

INSERT INTO `tblUsers` (`userCode`, `userName`, `pass`) VALUES
('tech2', 'tech2', 'tech2'),
('tech', 'tech', 'tech');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tblCalls`
--
ALTER TABLE `tblCalls`
  ADD PRIMARY KEY (`code`);

--
-- Indexes for table `tblRep`
--
ALTER TABLE `tblRep`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tblSuppParts`
--
ALTER TABLE `tblSuppParts`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tblUsers`
--
ALTER TABLE `tblUsers`
  ADD PRIMARY KEY (`userCode`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tblRep`
--
ALTER TABLE `tblRep`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=12;
--
-- AUTO_INCREMENT for table `tblSuppParts`
--
ALTER TABLE `tblSuppParts`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
