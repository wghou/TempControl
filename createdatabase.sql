/*
Navicat MySQL Data Transfer

Source Server         : ocean
Source Server Version : 50505
Source Host           : 127.0.0.1:3306
Source Database       : autotest

Target Server Type    : MYSQL
Target Server Version : 50505
File Encoding         : 65001

Date: 2020-05-06 20:50:47
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for s_instrument
-- ----------------------------
DROP TABLE IF EXISTS `s_instrument`;
CREATE TABLE `s_instrument` (
  `vInstrumentID` varchar(255) NOT NULL COMMENT '测试项编号',
  `vTestID` varchar(50) NOT NULL,
  `vCustomer` varchar(255) DEFAULT NULL COMMENT '送检客户',
  `vDesignation` varchar(255) DEFAULT NULL COMMENT '仪器名称',
  `vSpecification` varchar(255) DEFAULT NULL COMMENT '型号规格',
  `vSN` varchar(255) DEFAULT NULL COMMENT '出厂编号',
  `vManufacture` varchar(255) DEFAULT NULL COMMENT '制造单位',
  `vTestItem` varchar(255) DEFAULT NULL COMMENT ' 测试项目',
  `vTestType` varchar(255) DEFAULT NULL COMMENT '测试类型',
  PRIMARY KEY (`vInstrumentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of s_instrument
-- ----------------------------
INSERT INTO `s_instrument` VALUES ('3633b6d4-2066-4eda-b6bf-9ed026f957a1', '20200401-220155', '国家海洋技术中心', 'CTD', 'SBE 37', '5678', '美国海鸟公司', null, '计量站TCS校准');
INSERT INTO `s_instrument` VALUES ('419f953c-dea3-4fff-aab1-bc9d193271a8', '20200401-220155', '国家海洋技术中心', 'CTD', 'SBE 37', '1', '美国海鸟公司', null, '计量站TCS校准');

-- ----------------------------
-- Table structure for s_instrumentdata
-- ----------------------------
DROP TABLE IF EXISTS `s_instrumentdata`;
CREATE TABLE `s_instrumentdata` (
  `vTestID` varchar(50) DEFAULT NULL COMMENT '测试编号',
  `vInstrumentID` varchar(50) DEFAULT NULL COMMENT '仪器编号',
  `vItemType` varchar(255) DEFAULT NULL COMMENT '项目类型T/C',
  `vTemperature` double DEFAULT NULL COMMENT '电导率对应的温度值',
  `vTitularValue` int(11) DEFAULT NULL COMMENT '名义值',
  `vRealValue` double DEFAULT NULL COMMENT '平均值',
  `vRawValue` double DEFAULT NULL COMMENT ' 原始值',
  `vData` text COMMENT '某个校准点仪器所有数据',
  `vAddTime` datetime DEFAULT NULL,
  `vUpdateTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of s_instrumentdata
-- ----------------------------

-- ----------------------------
-- Table structure for s_sensor
-- ----------------------------
DROP TABLE IF EXISTS `s_sensor`;
CREATE TABLE `s_sensor` (
  `vSensorID` varchar(50) NOT NULL,
  `vInstrumentID` varchar(50) DEFAULT NULL,
  `vSensorName` varchar(255) DEFAULT NULL,
  `vSensorSN` varchar(255) DEFAULT NULL,
  `vSensorType` varchar(255) DEFAULT NULL,
  `vTestItem` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`vSensorID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of s_sensor
-- ----------------------------
INSERT INTO `s_sensor` VALUES ('62b9a547-d413-42c1-b1df-e1d3e17a6ff6', '3633b6d4-2066-4eda-b6bf-9ed026f957a1', '电导率', '1186', 'C', '电导率');
INSERT INTO `s_sensor` VALUES ('af01f0d6-0f77-4e6d-9dfc-0a96e3b95c9e', '419f953c-dea3-4fff-aab1-bc9d193271a8', '温度传感器', '1', 'T', '温度');
INSERT INTO `s_sensor` VALUES ('b416d991-6ccd-47ec-bf09-a3b1c01e0e5d', '419f953c-dea3-4fff-aab1-bc9d193271a8', '电导率传感器', '2', 'C', '电导率');
INSERT INTO `s_sensor` VALUES ('c818c760-89ba-4f9a-8638-17af9c1be8d2', '3633b6d4-2066-4eda-b6bf-9ed026f957a1', '温度', '1185', 'T', '温度');

-- ----------------------------
-- Table structure for s_standarddata
-- ----------------------------
DROP TABLE IF EXISTS `s_standarddata`;
CREATE TABLE `s_standarddata` (
  `vTestID` varchar(50) DEFAULT NULL COMMENT '测试编号',
  `vTitularValue` int(11) DEFAULT NULL COMMENT '温度名义值',
  `vStandardT` double DEFAULT NULL COMMENT '标准温度',
  `vStandardC` double DEFAULT NULL COMMENT '标准电导率',
  `vMeasureTime` datetime DEFAULT NULL COMMENT '测量时间',
  `vAddTime` datetime DEFAULT NULL,
  `vUpdateTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of s_standarddata
-- ----------------------------

-- ----------------------------
-- Table structure for s_testorder
-- ----------------------------
DROP TABLE IF EXISTS `s_testorder`;
CREATE TABLE `s_testorder` (
  `vTestID` varchar(50) NOT NULL COMMENT ' 测试编号',
  `vStartTime` datetime DEFAULT NULL COMMENT '开始时间',
  `vEndTime` datetime DEFAULT NULL COMMENT '结束时间',
  `vStatus` varchar(255) DEFAULT NULL COMMENT '测试状态',
  `vPlace` varchar(255) DEFAULT NULL,
  `vTemperature` float unsigned zerofill DEFAULT NULL COMMENT '温度',
  `vHumidity` float unsigned zerofill DEFAULT NULL COMMENT '相对湿度',
  `vCharger` varchar(255) DEFAULT NULL COMMENT '负责人',
  `vAddTime` datetime DEFAULT NULL,
  `vUpdateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`vTestID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of s_testorder
-- ----------------------------
INSERT INTO `s_testorder` VALUES ('20200401-220155', '2020-04-03 22:01:55', '2020-04-04 00:51:41', null, '海水温度校准室', '0000000020.5', '0000000052.1', '沈飞飞', null, null);
